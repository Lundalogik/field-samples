param(
  [parameter(mandatory=$true)]
  $href,
  [parameter(mandatory=$false)]
  [int] $limit = 0,
  [parameter(mandatory=$false, valuefrompipeline=$true, helpmessage="API client created using get-apiclient.ps1")]
  $apiclient = $__limecrm_apiclient
)

function ExpandSingleNoteProperty( $obj ) {
  $noteProperties = $obj | Get-Member -MemberType NoteProperty
  if( ( $noteProperties | Measure ).Count -eq 1 ) {
    $propertyName = $noteProperties.Name
    Write-Host "Auto-expanding response to property $propertyName"
    $next = $obj.$propertyName
    ExpandSingleNoteProperty $next
  } elseif( ( $noteProperties | ?{ $_.Name -eq "_embedded" } ) ) {
    ExpandSingleNoteProperty $obj._embedded
  } else {
    $obj
  }
}

if( -not $href.EndsWith("/") ) {
  $href += "/"
}
$query = @{}
if( $limit -gt 0 ) {
  $query.Add( "_limit", $limit )
}

$response = $apiclient.getJson( $href, $query )
ExpandSingleNoteProperty $response