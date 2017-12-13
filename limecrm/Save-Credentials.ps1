param(
  [parameter(mandatory=$true, valuefrompipelinebypropertyname=$true)]
  [string] $LimeCrmApplicactionUrl,
  [parameter(mandatory=$true, valuefrompipelinebypropertyname=$true)]
  [string] $ApiKey,
  [parameter(mandatory=$false, valuefrompipelinebypropertyname=$true)]
  [switch] $DisableSSLValidation,
  [parameter(mandatory=$true, helpmessage="Path to the credentials file to write to")]
  $path
)

$props = @{
  LimeCrmApplicactionUrl = $LimeCrmApplicactionUrl;
  ApiKey = $ApiKey
}
if( $DisableSSLValidation ) {
  $props.Add( "DisableSSLValidation", $true )
}
$credentialsJson = new-object psobject -property $props | ConvertTo-Json

Set-Content -Encoding UTF8 -Path $path -Value $credentialsJson
Write-Host -ForegroundColor yellow "Credentials saved to $path"