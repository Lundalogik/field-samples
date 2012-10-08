param( 
	[parameter(mandatory=$true)]
	[ValidateScript({ $_ -imatch "^https?://.+/api/?$" })]
	$ServiceUri,
	[parameter(mandatory=$true)]
	$Username,
	[parameter(mandatory=$true)]
	$Password
)

function curlGet( $ServiceUri, $Username, $Password, $href ) {
	$cred = "{0}:{1}" -f $Username, $Password
	[xml](curl -s -u $cred -H 'Accept: application/xml' $ServiceUri/$href)
}

# Save credentials
$env:RXA_COMMANDS_CS = "{0}`t{1}`t{2}" -f $ServiceUri,$Username,$Password | ConvertTo-SecureString -AsPlainText -Force | ConvertFrom-SecureString

$availableCommands = curlGet $ServiceUri $Username $Password "commands"
$availableCommands.AvailableCommands.Commands.Command
