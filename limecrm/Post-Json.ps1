param(
  [parameter(mandatory=$true)]
  $href,
  [parameter(mandatory=$true)]
  $payload,
  [parameter(mandatory=$false, valuefrompipeline=$true, helpmessage="API client created using get-apiclient.ps1")]
  $apiclient = $__limecrm_apiclient
)

$response = $apiclient.postJson( $payload, $href ) | % Response
if( $response ) {
    $response | convertfrom-json
}
