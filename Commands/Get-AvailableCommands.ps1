param( 
	[parameter(mandatory=$true)]
	[ValidateScript({ $_ -imatch "^https?://.+/api/?$" })]
	$ServiceUri,
	[parameter(mandatory=$true)]
	$Username,
	$Password = (Read-Host -Prompt "Enter password" -AsSecureString)
)

function curlGet( $ServiceUri, $Username, $Password, $href ) {
	$cred = "{0}:{1}" -f $Username, $Password
	[xml](curl -s -u $cred -H 'Accept: application/xml' $ServiceUri/$href)
}

# Save credentials
if( $Password -is [System.Security.SecureString] ) {
	$Password = [System.Runtime.InteropServices.marshal]::PtrToStringAuto([System.Runtime.InteropServices.marshal]::SecureStringToBSTR( $password ) )
}

$env:RXA_COMMANDS_CS = "{0}`t{1}`t{2}" -f $ServiceUri,$Username,$Password | ConvertTo-SecureString -AsPlainText -Force | ConvertFrom-SecureString

$availableCommands = curlGet $ServiceUri $Username $Password "commands"
$availableCommands.AvailableCommands.Commands.Command
