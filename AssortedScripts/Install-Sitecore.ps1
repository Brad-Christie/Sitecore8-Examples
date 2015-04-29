# TODO: Possibly integrate this will the installation of Sitecore as well
# https://github.com/reyrahadian/sitecore-installer ?

function Module-IsLoaded
{
  param(
    # module name
    [Parameter(Mandatory = $true, Position = 0)]
    [string] $name
  )
  Get-Module -Name $name
}
function Module-Exists
{
  param(
    # module name
    [Parameter(Mandatory = $true, Position = 0)]
    [string] $name
  )
  Get-Module -ListAvailable | Where-Object { $_.Name -eq $name }
}

function Unpack-Site
{
  <#
  .SYNPOSIS
  Extracts the Sitecore archive to the instance's directory.
  
  .DESCRIPTION
  Sets up the website folder and extracts the archive of the
  Sitecore directory into the specified instance folder.
  
  .PARAMETER siteRoot
  Root path of the new instance.
  
  .PARAMETER archice
  Full path to the archive to use.
  
  .EXAMPLE
  Extract-Site "C:\inetpub\wwwroot\MySite" "C:\Sitecore\Sitecore 8.0 rev. 150223.zip"
  
  Established the C:\inetpub\wwwroot\MySite folder and extracts the
  archive inside.
  
  .NOTES
  This function should be run with elevated privileges.
  #>
  Param(
    # Name of the instance you're looking to install to
    [Parameter(Mandatory = $true)]
    [string] $siteRoot,

    # Zipped Sitecore folder
    [Parameter(Mandatory = $true)]
    [string] $archive
  )

  # Validate the source archive exists
  if (!(Test-Path $archive))
  {
    Write-Error "Archive does not exist."
    Exit 1
  }

  # begin building instances (if it doesn't already exist)
  if (!(Test-Path $siteRoot))
  {
    # create the directory in inetpub
    New-Item $siteRoot -ItemType Directory -Force | Out-Null
    
    # Make sure the standard "Website" folder doesn't already exist
    if (!(Test-Path (Join-Path $siteRoot -ChildPath "Website")))
    {
      $shellApp = New-Object -Com "Shell.Application"
      $zipName = [IO.Path]::GetFileNameWithoutExtension($archive)
      $zipSrc = $shellApp.Namespace($archive)
      $zipDest = $shellApp.Namespace($siteRoot)
      # Note: We want the inner content folder of the archive
      # so we don't extract to \inetpub\wwwroot\instancename\zipname\Website but to
      # \inetpub\wwwroot\instancename\Website instead.
      $zipContent = ($zipSrc.Items() | Where-Object { ([string]$_.Name) -eq $zipName }).GetFolder.Items()
      $zipDest.CopyHere($zipContent)
    }
  }
}

function Add-Site
{
  <#
  .SYNPOSIS
  Adds a Site to IIS using the supplied parameters.
  
  .DESCRIPTION
  The Add-Site function uses the WebAdministration module to establish a
  website within IIS using the supplied parameters. The only required
  parameter is the $path parameter (and others are inferred based on that).
  
  .PARAMETER path
  The path to the directory to be used as the Site's root.
  
  .PARAMETER siteName
  The name of the Site, as it should appear, in IIS.
  This value, when absent, is defaulted to the directory name of path. For
  example, a path of '\inetpub\wwwroot\mysite' would result in a siteName
  of 'mysite'.
  
  .PARAMETER appPoolName
  The name of the AppPool, as it should appear, in IIS.
  This value, when absent, is defaulted to the siteName.
  
  .PARAMETER hostName
  The hostname that should be added to the bindings of the Site.
  This value, when absent, is defaulted to the siteName.
  
  .EXAMPLE
  Add-Site "C:\inetpub\wwwroot\mysite"
  
  Adds the AppPool "mysite" with the Site "mysite" bound to the hostname
  "mysite"
  
  .EXAMPLE
  Add-Site "C:\inetpub\wwwroot\othersite" -hostName othersite.local
  
  Adds the AppPool "othersite" with the Site "othersite" bound to the
  hostnames "othersite" and "othersite.local".
  
  .NOTES
  This function should be run with elevated privileges. And if you
  do not have the Carbon module installed, you will need to modify
  (add) the HOSTS file entries yourself.
  #>
  param(
    [string] $siteName = $null,
    [string] $appPoolName = $null,
    [string] $hostName = $null,
    
    [Parameter(Mandatory = $true, Position = 0)]
    [string] $path
  )
  
  # Establish default values
  # @#$$#%#$@#. Why does powershell have to handle $null values in a stupid manner.
  # http://www.zeninteractions.com/2013/10/17/checking-for-null-in-powershell/
  if ($siteName -eq "" -and $siteName -eq [String]::Empty)
  {
    $siteName = [IO.Path]::GetFileName($path)
  }
  if ($appPoolName -eq "" -and $appPoolName -eq [String]::Empty)
  {
    $appPoolName = $siteName
  }
  if ($hostName -eq "" -and $hostName -eq [String]::Empty)
  {
    $hostName = $siteName
  }
  
  Write-Host "siteName = $($siteName)"
  Write-Host "path = $($path)"
  Write-Host "appPoolName = $($appPoolName)"
  Write-Host "hostName = $($hostName)"
  return
  
  # Create directory
  New-Item -ItemType Directory -Force -Path $path | Out-Null
  # May also want to copy ACL permissions? Anything the AppPool requires should be
  # added here.
  # More info: https://confidentialfiles.wordpress.com/2014/03/13/copying-ntfs-permissions-between-folders/
  #Get-Acl -Path "C:\inetpub\wwwroot\OtherWebsite" | Set-Acl -Path $path

  # We need WebAdministration to work with IIS
  if (!(Module-Exists "WebAdministration"))
  {
    Write-Error ="WebAdministration module required."
    Exit 1
  }
  if (!(Module-IsLoaded "WebAdministration"))
  {
    Import-Module "WebAdministration"
  }
  
  # Create the AppPool
  Push-Location IIS:\AppPools\
  if (!(Test-Path $appPoolName -PathType Container))
  {
    $appPool = New-Item $appPoolName
    $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value "v4.0"
    Write-Host "Created AppPool $($appPoolName)"
  }
  Pop-Location

  # Create the Site
  Push-Location IIS:\Sites\
  if (!(Test-Path $siteName -PathType Container))
  {
    $site = New-Item $siteName -bindings @{protocol="http";bindingInformation="*:80:$($siteName)"} -PhysicalPath $path
    $site | Site-ItemProperty -Name "applicationPool" -Value $appPoolName
    if ($siteName -ne $hostName)
    {
      #New-ItemProperty $site.Name -Name bindings -Value @{protocol="http";bindingInformation="*:80:$($hostName)"}
      # New-WebBinding :: http://technet.microsoft.com/en-us/library/ee790567.aspx
      New-WebBinding -Name $site.Name -IPAddress "*" -Port 80 -HostHeader $hostName
    }
    Start-Website -Name $site.Name 
    Write-Host "Created Site $($siteName)"
  }
  Pop-Location
  
  # Have Carbon? (http://get-carbon.org/)
  if (Module-Exists "Carbon")
  {
    if (!(Module-IsLoaded "Carbon"))
    {
      Import-Module "Carbon"
    }
    Set-HostsEntry -IPAddress 127.0.0.1 -HostName $hostName -Description $siteName
    Write-Host "Added HOSTS entry for $($hostName)"
  }
  
  Write-Host "Created $($siteName) at http://$($hostName)/"
}

# Example Usage:
$iisRoot = "C:\inetpub\wwwroot\"
Unpack-Site (Join-Path $isRoot -ChildPath "MySitecore") "C:\Sitecore 8.0 rev. 150223.zip"
Add-Site (Join-Path $iisRoot -ChildPath "MySitecore") -hostName my.sitecore.local