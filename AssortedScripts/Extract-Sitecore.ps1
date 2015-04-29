[CmdletBinding()]
Param(
    # Name of the instance you're looking to install to
    [Parameter(Mandatory = $true)]
    [string] $name,

    # Zipped Sitecore folder
    [Parameter(Mandatory = $true)]
    [string] $archive
)
#$DebugPreference = "Continue"
#$VerbosePreference = "Continue"

Write-Debug "Name = $($name), Archive = $($archive)"

# Check for source archive
if (!(Test-Path $archive))
{
    Write-Error "Archive does not exist."
    Exit 1
}

# Load required modules
Import-Module WebAdministration | Out-Null

# Initial setup
$siteName = $name
$appPoolName = "$($siteName)AppPool"
$siteRoot = (Join-Path "C:\inetpub\wwwroot\" -ChildPath $siteName)
Write-Debug "siteName = $($siteName), siteRoot = $($siteRoot), appPoolName = $($appPoolName)"

if (!(Test-Path $siteRoot))
{
    New-Item $siteRoot -ItemType Directory -Force | Out-Null
    $siteRootWebsite = (Join-Path $siteRoot -ChildPath "Website")
    if (!(Test-Path $siteRootWebsite))
    {
        $shellApp = New-Object -Com "Shell.Application"
        $zipName = [IO.Path]::GetFileNameWithoutExtension($archive)
        $zipSrc = $shellApp.Namespace($archive)
        $zipDest = $shellApp.Namespace($siteRoot)
        $zipContent = ($zipSrc.Items() | Where-Object { ([string]$_.Name) -eq $zipName }).GetFolder.Items()
        $zipDest.CopyHere($zipContent)
    }
}