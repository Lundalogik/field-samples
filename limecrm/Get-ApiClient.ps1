param(
  [parameter(mandatory=$true, valuefrompipelinebypropertyname=$true)]
  [string] $LimeCrmApplicactionUrl,
  [parameter(mandatory=$true, valuefrompipelinebypropertyname=$true)]
  [string] $ApiKey,
  [parameter(mandatory=$false, valuefrompipelinebypropertyname=$true)]
  [switch] $DisableSSLValidation
)
process {
  if( $DisableSSLValidation ) {
    Write-Warning "Disabling SSL validation for this session of Powershell"
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true};
  }

  $webClient = New-Object System.Net.WebClient
  $webClient.Encoding = [System.Text.Encoding]::Utf8
  $headers = @{}
  $headers.Add( "x-api-key", $ApiKey)
  $headers.Add( "Accept", "application/hal+json")
  $headers.Add( "Content-Type", "application/json; charset=utf-8")

  function makeApiUrl {
    param( $relativeOrAbsoluteUrl )

    function trailingSlash {
      param( $url )
      if( -not $url.EndsWith("/") -and -not $url.Contains("?") ) {
        "{0}/" -f $url
      } else {
        $url
      }
    }

    if( $relativeOrAbsoluteUrl.StartsWith( $this.LimeCrmApplicactionUrl ) ) {
      trailingSlash $relativeOrAbsoluteUrl
    } else {
      trailingSlash ("{0}/api/v1/limeobject/{1}" -f $this.LimeCrmApplicactionUrl, $relativeOrAbsoluteUrl)
    }
  }
  function getJson {
    param( $url, [hashtable] $queryHashtable )
    $absoluteUrl = $this.makeApiUrl( $url )
    # WebClient headers are reset now and then and needs to be added before each request
    $this.Headers.Keys | %{
      $this.WebClient.Headers[$_] = $this.Headers[$_]
    }
    if( $queryHashtable -and $queryHashtable.Count ) {
      if( !$absoluteUrl.Contains("?") ) {
        $absoluteUrl += "?"
      }
      $absoluteUrl += [string]::Join( "&", ($queryHashtable.Keys | %{ "{0}={1}" -f $_, $queryHashtable[$_] }) )
    }
    try{
    $json = $this.WebClient.DownloadString( $absoluteUrl )
    $json | ConvertFrom-Json `
    | Add-Member -membertype ScriptProperty -name "AbsoluteHref" -value ([ScriptBlock]::Create("return ""$absoluteUrl""")) -passthru
    } catch [Exception]{
      $_ | select * | out-host
    }
  }

  function sendJson {
    param( $dto, $method, $href )
    if( -not $href ) {
      $href = $dto.Href
    }
    $absoluteUrl = $this.makeApiUrl( $href)
    $json = $dto | convertto-json -compress -depth 100  | out-string
    $result = new-object psobject -property @{ AbsoluteHref = $absoluteUrl; Success = $false; Payload = $json; Response = (new-object psobject); Error = $null }
    try {
      # WebClient headers are reset now and then and needs to be added before each request
      $this.Headers.Keys | %{
        $this.WebClient.Headers[$_] = $this.Headers[$_]
      }
      $result.Response = $this.WebClient.UploadString( (new-object System.Uri $absoluteUrl), $method, $json )
      $result.Success = $true
    } catch {
      $result.Error = $_.Exception
      $result.Response = $_.Exception | % Response
    }
    $result
  }

  function putJson {
    param( $dto, $href )
    $this.sendJson( $dto, "PUT", $href )
  }

  function postJson {
    param( $dto, $href )
    $this.sendJson( $dto, "POST", $href )
  }

  function deleteJson {
    param( $dto, $href )
    $this.sendJson( $dto, "DELETE", $href )
  }

  function delete {
    param( $href )
    $this.sendJson( $null, "DELETE", $href )
  }

  $apiClient = new-object psobject -property @{ Headers = $headers; WebClient = $webClient; LimeCrmApplicactionUrl = $LimeCrmApplicactionUrl }

  function addMember {
    param( $name, $type = "ScriptMethod" )
    $apiClient | Add-Member -membertype $type -name $name -value (get-command $name | % ScriptBlock)
  }

  addMember "makeApiUrl"
  addMember "getJson"
  addMember "sendJson"
  addMember "putJson"
  addMember "postJson"
  addMember "delete"
  addMember "deleteJson"

  $aliases = "get-json","post-json","put-json"
  Write-Host "Creating command aliases: $aliases"
  $aliases | %{
    $scriptpath = join-path $PSScriptRoot "$($_).ps1"
    Set-Alias -Name $_ -Value $scriptpath -Option AllScope
  }

  $apiClient | & $PSScriptRoot\Set-CurrentApiClient.ps1

  $apiClient
}
