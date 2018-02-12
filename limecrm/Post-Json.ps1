param(
  [parameter(mandatory=$true)]
  $payload,
  [parameter(mandatory=$true)]
  [string] $href,
  [parameter(mandatory=$false, valuefrompipeline=$true, helpmessage="API client created using get-apiclient.ps1")]
  $apiclient = $__limecrm_apiclient
)

$response = $apiclient.postJson( $payload, $href )
if( $response.Success ) {
  $response.Response | convertfrom-json
} else {
  Write-Error $response
}
