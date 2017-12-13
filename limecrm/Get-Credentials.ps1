param(
  [parameter(mandatory=$true, parametersetname="byargs")]
  [string] $LimeCrmApplicactionUrl,
  [parameter(mandatory=$true, parametersetname="byargs")]
  [string] $ApiKey,
  [parameter(mandatory=$false, parametersetname="byargs")]
  [switch] $DisableSSLValidation,
  [parameter(mandatory=$true, parametersetname="byfile")]
  $path
)

if( $PSCmdlet.ParameterSetName -eq "byfile" ) {
  if( -not (test-path $path) ) {
    throw "The file at '$path' does not exist"
  }
  $credentials = Get-Content -Encoding UTF8 -Path $path | ConvertFrom-Json
  if( -not ( $credentials.LimeCrmApplicactionUrl -and $credentials.ApiKey) ) {
    throw "The file at '$path' does not seem to be a credentials file"
  }
  $LimeCrmApplicactionUrl = $credentials.LimeCrmApplicactionUrl
  $ApiKey = $credentials.ApiKey
  if( $credentials.DisableSSLValidation ) {
    $DisableSSLValidation = $true
  }
}

$props = @{
  LimeCrmApplicactionUrl = $LimeCrmApplicactionUrl;
  ApiKey = $ApiKey
}

if( $DisableSSLValidation ) {
  $props.Add( "DisableSSLValidation", $true )
}

new-object psobject -property $props