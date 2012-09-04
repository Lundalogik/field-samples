param( 
	[parameter(mandatory=$true)]
	$excelFile,
	[parameter(mandatory=$true)]
	$sheetName,
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
					  -reencodefromencoding UTF7
	if( !(gi $commandBatchPath) ) {
		Write-Error "Command batch file $commandBatchPath could not be found"
		exit 1
	}
	executeCommands -Path $commandBatchPath `
					-serviceuri $serviceUri `
					-username $username `
					-password $password
	if(!$?) {
		Write-Error "Failed to execute command batch $commandBatchPath"
		exit 1
	}
}
	