<#
.SYNOPSIS
	Imports data to RemoteX using the command batches API
.DESCRIPTION
	Uses either Excel or CSV files as the data source and imports the data to RemoteX
	using the command batches API. Microsoft Excel needs to be installed if an Excel
	file is used as the data source.
	
	To selectively import data there is two options to constrain the amount of data 
	sent to the API. The sheetName parameter will select the sheet of the Excel file.
	The rowFilter parameter can be used both with CSV files and Excel files and will
	be applied to each record found in the data source.
	I.e. if there is a column named "Active Record" in the data source the following 
	filter can be used to select only those rows whose cell value is "True":
	-rowFilter { $_."Active Record" -eq "True" }
	
.PARAMETER excelFile
	The source Excel file
.PARAMETER sheetName
	Required. The name of the sheet whose data is to be imported
.PARAMETER csvFiles
	Used when the data source is CSV instead of any Excel file. Office is not required to be installed.
.PARAMETER rowFilter
	A row filter to apply to the source file before processing any rows
.PARAMETER commandName
	The command name to be used for each CSV record if not specified in the CommandName column
.PARAMETER serviceUri
	The URL to the API endpoint.
.PARAMETER username
	Username to authenticate with
.PARAMETER password
	Password to authenticate with
.PARAMETER chunkSize
	The maximum size of each command batch
.PARAMETER continueOnError
	When a command fails, default mode is to reject all commands sent to the API.
	This option changes this and will persist all changes made by commands that executed successfully.
	
#>
param( 
	[parameter(mandatory=$true, parametersetname="Excel")]
	$excelFile,
	[parameter(mandatory=$true, parametersetname="Excel")]
	[String[]] $sheetName,
	[parameter(mandatory=$true, parametersetname="CSV", valuefrompipeline=$true)]
	$csvFiles,
	[parameter(mandatory=$false)]
	[scriptblock] $rowFilter = $null,
	[parameter(mandatory=$false)]
	$commandName,
	[parameter(mandatory=$true)]
	$serviceUri,
	[parameter(mandatory=$true)]
	$username,
	[parameter(mandatory=$true)]
	$password,
	[parameter(mandatory=$false)]
	[int] $chunkSize = 200,
	[parameter(mandatory=$false,helpmessage="Removes the delay between batches")]
	[Switch] $noDelay,
	[parameter(mandatory=$false,helpmessage="Time to wait between batches - a factor of the time taken for the last batch to complete")]
	[decimal] $delayFactor = 1,
	[parameter(mandatory=$false,helpmessage="Persist all successful commands even if some are failing")]
	[Switch] $continueOnError
)

	$ScriptDir = $MyInvocation.MyCommand.Path | split-path
	
Set-Alias excelToCsv $ScriptDir\Convert-ExcelToCsv.ps1
Set-Alias csvToCommandBatch $ScriptDir\Convert-CsvToCommandBatch.ps1
Set-Alias executeCommands $ScriptDir\Execute-Commands.ps1

if( $excelFile ) {
	$csvFiles = excelToCsv -excelFile $excelFile -filter $sheetName -noOutput:$false
	if(!$csvFiles) {
		Write-Error "No CSV files produced"
		exit 1
	}
}

$swtotal = [System.Diagnostics.Stopwatch]::StartNew()
$commandBatches = $csvFiles | %{
	$commandBatchPath = $_.Path -replace "\.csv$","_CommandBatch.xml"
	$commandBatchChunks = csvToCommandBatch -csvFile $_.Path `
					  -outputFile $commandBatchPath `
					  -defaultCommandName $commandName `
					  -filter $rowFilter `
					  -continueOnError:$continueOnError `
					  -chunkSize $chunkSize `
					  -encoding UTF7
					  
	if( !$commandBatchChunks ) {
		Write-Error "Command batch files were not created for $($_.Path). Empty file?"
		exit 1
	}
	$commandBatchChunks
}

$commandBatchCount = ($commandBatches | measure).Count
Write-Host "Data was split into $commandBatchCount chunks"
$commandBatchIndex = 0
$delay = 0
$commandBatches | %{
	$commandBatchPath = $_.Path
	$percentComplete = (100 * ( $commandBatchIndex / $commandBatchCount ))
	$commandBatchIndex++
	if( !$noDelay -and $delay -gt 0 ) {
		$secondsRemaining = (($commandBatchCount-$commandBatchIndex+1) * (($swtotal.Elapsed.TotalSeconds / $commandBatchIndex) + $delay))
		Write-Progress -Activity ("Sending commands to $serviceUri using $commandBatchPath ({0:f2}Mb in size)" -f ((gi $commandBatchPath).Length/1mb)) `
						-Status ("Waiting {0:f2} seconds before sending the next command batch..." -f $delay) `
						-SecondsRemaining $secondsRemaining `
						-PercentComplete $percentComplete
		sleep -Seconds $delay
	}
	$secondsRemaining = (($commandBatchCount-$commandBatchIndex+1) * (($swtotal.Elapsed.TotalSeconds / $commandBatchIndex) + $delay))
	Write-Progress -Activity ("Sending commands to $serviceUri using $commandBatchPath ({0:f2}Mb in size)" -f ((gi $commandBatchPath).Length/1mb)) `
					-Status "Sending..." `
					-SecondsRemaining $secondsRemaining `
					-PercentComplete $percentComplete
	$sw = [System.Diagnostics.Stopwatch]::StartNew()
	$cmderr = @()
	executeCommands -Path $commandBatchPath `
					-serviceuri $serviceUri `
					-username $username `
					-password $password `
					-nooutput `
					-errorvariable cmderr
	$delay = $sw.Elapsed.TotalSeconds * $delayFactor

	if($cmderr) {
		Write-Error "Failed to execute command batch ${commandBatchPath}: $cmderr"
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
		Write-Host "Stopped at chunk $commandBatchIndex of $commandBatchCount"
		exit 1
	}
}