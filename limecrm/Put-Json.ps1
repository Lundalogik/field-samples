param(
  [parameter(mandatory=$true, helpmessage="The limeobject to put")]
  $payload,
  [parameter(mandatory=$false, helpmessage="The relative limeobject url to use. Leave empty to use 'self' link from payload")]
  $href,
  [parameter(mandatory=$false, valuefrompipeline=$true, helpmessage="API client created using get-apiclient.ps1")]
  $apiclient = $__limecrm_apiclient
)

if( !$href -and -not $payload._links.self.href ) {
  throw "Could not figure out the URL to put to, please ensure that your payload has a 'self' link or specify the -href parameter"
}
if( !$href ) {
  $href = $payload._links.self.href
}

$response = $apiclient.putJson( $payload, $href )
if( $response.Success ) {
  $response.Response | convertfrom-json
} else {
  Write-Error $response
}
