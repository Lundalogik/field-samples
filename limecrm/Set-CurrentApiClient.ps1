param(
  [parameter(mandatory=$true, helpmessage="API klient skapad med get-apiclient.ps1", valuefrompipeline=$true)]
  $apiclient
)
$global:__limecrm_apiclient = $apiclient
