<#
.SYNOPSIS
	Creates a command batch from a CSV file as the data source for the command parameters.
.DESCRIPTION
	This script can be used to convert command batches from CSV to XML. 
	The input data file can contain the name of the command to carry out for each CSV record in
	the column named CommandName, but this is not neccessary. The script parameter -defaultCommandName
	can be used if the data source only contains data and no command names.
	
	Each column, except for CommandName, is mapped to command parameters. However, the parameter can
	be overridden in the file by specifying the target parameter name in square brackets - for example:
	
	Titel [Title],Benämning [Description],Annat []
	
	Note: Using an empty set of brackets ignores the column.
	
	If the source file can't be modified for column to parameter name mapping there are some 
	script parameters available to accomplish the same thing;
	-columnToParameterMap is specificed as a hash table and -ignoredColumns is a string array. 
	The column to parameter name mapping example above can be accomplished using the following:
	-columnToParameteMap @{ "Titel" = "Title"; "Benämning" = "Description" } 
	-ignoredColumns "Annat"
	
	If the file comes from Excel it may be saved using ASCII or UTF7 encoding. To convert the 
	data source from that encoding to UTF8 the following parameter can be used:
	-reencodeFromEncoding UTF7
	
	To preview the specified column to parameter name mapping without actually creating 
	an XML file, use the -preview switch.
#>
param( 
	[parameter(mandatory=$true, valuefrompipeline=$true)]
	$csvFile,
	[parameter(mandatory=$true)]
	$outputFile,
	[parameter(mandatory=$false)]
	$defaultCommandName = "",
	[parameter(mandatory=$false)]
	[hashtable]$columnToParameterMap,
	[parameter(mandatory=$false)]
	[string[]]$ignoredColumns,
	[parameter(mandatory=$false)]
	[Microsoft.PowerShell.Commands.FileSystemCmdletProviderEncoding] $reencodeFromEncoding,
	[parameter(mandatory=$false)]
	[Switch] $preview
)

$dataFile = $csvFile
if( $reencodeFromEncoding ) {
	$dataFile = [System.IO.Path]::GetTempFileName()
	gc $csvFile -Encoding $reencodeFromEncoding | Out-File -Encoding UTF8 -FilePath $dataFile
}
$data = Import-Csv $dataFile
Write-Host "Found $(($data | measure).Count) records"

if(!$columnToParameterMap) { $columnToParameterMap = @{} }
if(!$ignoredColumns) { $ignoredColumns = @() }

$xmlns = "http://schemas.remotex.net/Apps/201207/Commands"
$CommandBatch = [xml]@"
<?xml version="1.0" encoding="utf-8"?>
<CommandBatch xmlns="$xmlns" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
</CommandBatch>
"@

$attrs = $data | Select -first 1 | gm -MemberType NoteProperty | ?{ $_.Name -ne "CommandName" } | Select -ExpandProperty Name

function getParameterNameForColumn( $columnName ) {
	$columnName = $columnName.Trim()
	if( $ignoredColumns -contains $columnName ) {
		""
	} elseif( $columnToParameterMap.ContainsKey( $columnName ) ) {
		$columnToParameterMap[$columnName]
	} elseif( $columnName -match "(?<=\[)[^\]]*(?=\])" ) {
		$matches[0]
	} else {
		$columnName
	}
}

$parameterMap = $attrs | select @{Name="Column";Expression={ $_ }},@{Name="Parameter";Expression={ getParameterNameForColumn $_ }}
Write-Host "Parameter map:"
$parameterMap | ?{ $_.Parameter } | %{
	Write-Host ("  {0} => {1}" -f $_.Column, $_.Parameter)
}
Write-Host "Ignored columns:"
( $ignoredColumns + ( $parameterMap  | ?{ !$_.Parameter } | select -ExpandProperty Parameter ) ) | select -Unique | %{
	Write-Host "  $_"
}
Write-Host ""

if($preview) {
	return
}

foreach ($record in $data){
    $Command = $CommandBatch.CreateElement("Command", $xmlns)
    $Command.AppendChild($CommandBatch.CreateElement("Name", $xmlns)) | out-null
	if( $record.CommandName ) {
    	$Command.Name = $record.CommandName.ToString()
	} else {
		$Command.Name = $defaultCommandName
	}
    
    foreach ($attr in $attrs){
        $val = $record."$attr"
        if($val){
            $Parameter = $CommandBatch.CreateElement("Parameter", $xmlns)
            $Parameter.AppendChild($CommandBatch.CreateElement("Name", $xmlns)) | out-null
            $Parameter.Name = getParameterNameForColumn ($attr.ToString())
            $Parameter.AppendChild($CommandBatch.CreateElement("Value", $xmlns)) | out-null
            $Parameter.Value = $val.ToString().Trim()
            $Command.AppendChild($Parameter) | Out-Null
        }
    }
    $CommandBatch.CommandBatch.AppendChild($Command) | out-null
}

if( ![System.IO.Path]::IsPathRooted( $outputFile ) ) {
	$outputFile = Join-Path $PWD $outputFile
}
$CommandBatch.Save( $outputFile )