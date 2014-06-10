<#
.SYNOPSIS
    Executes commands created by using generated PowerShell functions.
.DESCRIPTION
	This script is used to send the commands created using generated PowerShell functions.
	Just execute the functions and pipe the result to this script.
.PARAMETER command
	The commands to be executed (from the pipeline)
#>
param( 
	[parameter(mandatory=$false)]
	[int] $chunkSize = 200,
	[parameter(mandatory=$false)]
	$batchId = "CommandBatch",
	[parameter(mandatory=$true, valuefrompipeline=$true)]
	$command
)
begin {
	$ScriptDir = $MyInvocation.MyCommand.Path | Split-Path

	$ConnectString = $null
	if($env:RXA_COMMANDS_CS) {
		$ConnectString = [System.Runtime.InteropServices.marshal]::PtrToStringAuto([System.Runtime.InteropServices.marshal]::SecureStringToBSTR( ( $env:RXA_COMMANDS_CS | ConvertTo-SecureString ) ) )
	}
	if($ConnectString) {
		$ServiceUri, $Username, $Password = $ConnectString.Split( "`t" )
	} else {
		Write-Error "Missing credentials - you must run get-availablecommands.ps1 before using Post-Commands"
		break
	}
	$xmlns = "http://schemas.remotex.net/Apps/201207/Commands"
	$commands = @()
	$cred = "{0}:{1}" -f $Username, $Password
	set-alias executeCommands $ScriptDir\Execute-Commands.ps1
	function postCommandBatch( $xmlDoc ) {
		$tempFile = [System.IO.Path]::GetTempFileName() -replace ".tmp$",".xml"
		$xmlText = $xmlDoc.Save( $tempFile )
		Write-Host ("Sending commands to $serviceUri using $commandBatchPath ({0:f2}Mb in size)" -f ((gi $tempFile).Length/1mb))
		executeCommands -path $tempFile -serviceuri $ServiceUri -username $Username -password $Password
	}
}
process {
	if( !$command.Name ) {
		Write-Error "Command has no name" -TargetObject $command
	} else {
		$commands += $command
	}
}
end {
	$chunks = [int]([Math]::Ceiling( ( $commands | measure ).Count / $chunkSize ))
	$chunkNumber = 0
	$currentBatchId = $batchId
	$recordNumber = 1
	$CommandBatch = $null
	foreach ($cmd in $commands ){
		$cmdNumber += 1
		if( !$CommandBatch ) {
			$chunkNumber += 1
			if( $chunks -gt 1 ) {
				$currentBatchId = "{0} - chunk {1}" -f $batchId.Trim(), $chunkNumber
			}
			$CommandBatch = [xml]@"
<?xml version="1.0" encoding="utf-8"?>
<CommandBatch xmlns="$xmlns" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
<Id>$currentBatchId</Id>
</CommandBatch>
"@
		}
		
	    $Command = $CommandBatch.CreateElement("Command", $xmlns)
	    $Command.AppendChild($CommandBatch.CreateElement("Name", $xmlns)) | out-null
    	$Command.Name = $cmd.Name.ToString()
		if( $cmd.Target ) {
	    	$Command.AppendChild($CommandBatch.CreateElement("Target", $xmlns)) | out-null
	    	$Command.Target = $cmd.Target.ToString()
		}
		
		$cmd.Parameters | gm -MemberType NoteProperty | select -ExpandProperty Name | %{
			$parameterName = $_
			$parameterValues = $cmd.Parameters."$_"
	        $Parameter = $CommandBatch.CreateElement("Parameter", $xmlns)
	        $Parameter.AppendChild($CommandBatch.CreateElement("Name", $xmlns)) | out-null
	        $Parameter.Name = $parameterName.ToString()
			$parameterValues | %{
		        $Parameter.AppendChild($CommandBatch.CreateElement("Value", $xmlns)) | out-null
		        $Parameter.Value = $_.ToString().Trim()
		        $Command.AppendChild($Parameter) | Out-Null
	        }
	    }
	    $CommandBatch.CommandBatch.AppendChild($Command) | out-null
		if( ( $recordNumber % $chunkSize ) -eq 0 ) {
			postCommandBatch $CommandBatch
			$CommandBatch = $null
		}
		$recordNumber++
	}
	if( $CommandBatch ) {	
		postCommandBatch $CommandBatch
	}
}
