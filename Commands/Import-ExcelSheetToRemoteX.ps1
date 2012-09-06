param( 
	[parameter(mandatory=$true)]
	$excelFile,
	[parameter(mandatory=$true)]
	$sheetName,
	[parameter(mandatory=$false)]
	[scriptblock] $rowFilter = { $true },
	[parameter(mandatory=$true)]
	$commandName,
	[parameter(mandatory=$true)]
	$serviceUri,
	[parameter(mandatory=$true)]
	$username,
	[parameter(mandatory=$true)]
	$password
)

if( !(Get-Command -ErrorAction SilentlyContinue Convert-ExcelToCsv.ps1) ) {
	Write-Error "Convert-ExcelToCsv.ps1 must be in the path"
	exit 1
}
if( !(Get-Command -ErrorAction SilentlyContinue Convert-CsvToCommandBatch.ps1) ) {
	Write-Error "Convert-CsvToCommandBatch.ps1 must be in the path"
	exit 1
}
if( !(Get-Command -ErrorAction SilentlyContinue Execute-Commands.ps1) ) {
	Write-Error "Convert-CsvToCommandBatch.ps1 must be in the path"
	exit 1
}

Set-Alias excelToCsv (Get-Command Convert-ExcelToCsv.ps1)
Set-Alias csvToCommandBatch (get-command Convert-CsvToCommandBatch.ps1)
Set-Alias executeCommands (get-command Execute-Commands.ps1)

$csvFiles = excelToCsv -excelFile $excelFile -filter $sheetName -noOutput:$false
if(!$csvFiles) {
	Write-Error "No CSV files produced"
	exit 1
}
$csvFiles | %{
	$commandBatchPath = $_.Path -replace "\.csv$","_CommandBatch.xml"
	csvToCommandBatch -csvFile $_.Path `
					  -outputFile $commandBatchPath `
					  -defaultCommandName $commandName `
					  -filter $rowFilter `
					  -reencodefromencoding UTF7
	if( !(gi $commandBatchPath) ) {
		Write-Error "Command batch file $commandBatchPath could not be found"
		exit 1
	}
	
	Write-Host ("Sending commands to $serviceUri ({0:f2}Mb in size)" -f ((gi $commandBatchPath).Length/1mb))
	executeCommands -Path $commandBatchPath `
					-serviceuri $serviceUri `
					-username $username `
					-password $password `
					-nooutput
	if(!$?) {
		Write-Error "Failed to execute command batch $commandBatchPath"
		$outputfile = $commandBatchPath -replace "\.xml$","_output.xml"
		$ErrorReport = $commandBatchPath -replace "\.xml$","_errors.csv"
		if( Test-Path $outputfile ) {
			Write-Host "Creating error report $errorReport"
			try {
				$xml = [xml](gc $outputfile)
				$err = $xml.CommandBatchResponse.CommandResponse | ?{ $_.HasErrors -eq "true" }  | %{ 
					$command = $_ | select ErrorMessage
					$_.Command.Parameter | ?{ $_.Name } | %{ 
						$command | add-member -membertype noteproperty -name $_.Name -valu $_.Value 
					}
					$command 
				} 
				$err | Export-Csv -NoTypeInformation -Path $ErrorReport -Encoding utf8
				$err 				
			} catch {
				Write-Error "Error creating error report! $_"
			}
		}
		exit 1
	}
}
	