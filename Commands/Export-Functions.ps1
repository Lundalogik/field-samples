<#
.SYNOPSIS
	Exports available commands as PowerShell functions.
.DESCRIPTION
	Use with Get-AvailableCommands.ps1 to add new functions to your PowerShell session.
#>
process {
	if( !$_ ) { continue }

	# Create function name like WriteToLog => Write-ToLog
	$commandName = $_.CommandName
	$functionName = $commandName -creplace "^([A-Z][^A-Z]*)([A-Z])",'$1-$2'
	$count = ($_.Parameters.Parameter | measure).Count
	$paramList = "`t[parameter(helpmessage='Enter the relative href of the target for the command')]`r`n`t[string] `$target"
	$helpParamList = ".PARAMETER target`r`n`tRelative href of the target for the command. Mandatory for most update commands.`r`n"
	$exampleParamList = ""
	$settersList = ""
	$i = 0
	foreach( $p in $_.Parameters.Parameter ) {
		$varName = $p.ParameterName -replace "[\.\s\-]", ""
		$description = ($p.Description -replace "[\r\n]","")
		$helpParamList += ".PARAMETER {0}`r`n`t{1}`r`n" -f $varName, $p.Description
		$paramList += ",`r`n`t[parameter(helpmessage=""{0}"")]`r`n`t[string[]] `${1}" -f $description, $varName
		
		if( $i -lt 5 ) {
			$exampleParamList += "-{0} ""value{1}"" " -f $varName, $i
		}
		
		if( $p.DefaultValue ) {
			$paramList += " = '{0}'" -f $p.DefaultValue
		} 

		$settersList += "if( `${1} ) {{ `$parameters.Add( ""{0}"", `${1} ) }}`r`n" -f $p.ParameterName, $varName
		$i++
	}
@"
<#
.SYNOPSIS
	Creates a $commandName command using the specified command parameters.
$helpParamList
.EXAMPLE
	PS> .\$functionName.ps1 -Target href/of/entity $exampleParamList
	
	Creates a command with the specified parameters.
#>
param(`r`n
$paramList
)
`$parameters = @{}
$settersList
new-object psobject -property @{ "Name" = "$commandName"; "Target" = `$target; Parameters = (new-object psobject -property `$parameters) }
"@ | sc -Encoding UTF8 -Path "$functionName.ps1"
	Write-Host -ForegroundColor Yellow "Exported $functionName.ps1. Syntax:"
	Get-Help "$functionName.ps1" | select -ExpandProperty syntax | Out-Host
}
