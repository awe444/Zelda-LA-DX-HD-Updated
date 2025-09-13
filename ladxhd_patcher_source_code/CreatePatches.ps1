#========================================================================================================================================
# LINK'S AWAKENING DX HD: XDELTA PATCH CREATOR FOR GAME PATCHER
# By: Bighead
#========================================================================================================================================
# PURPOSE
#========================================================================================================================================
<#

  The purpose of this script is to generate patches from v1.0.0 of Link's Awakening DX HD and whatever the latest version is. The
  original release should be set to the "$OldGamePath" variable, the new release set to the "$NewGamePath" variable, and the version
  should be set to the "$GameVersion" variable. After that, just run the script and it will generate all the necessary patches. From
  there the patches can be imported into "Resources.resx" of the patcher source code and compiled into the resulting patcher.

#>
#========================================================================================================================================
# INSTRUCTIONS
#========================================================================================================================================
<#

  Information:
  - Generate xdelta patches to update v1.0.0 or v1.1.4+ to the latest build.
  - XDelta3 patches must share a name with the file they are patching + ".xdelta" extension.
  - For example, the file "musicOverworld.data" the patch should be "musicOverworld.data.xdelta"

  Requirements:
  - Original v1.0.0 of the game.
  - New build of the game.
  - Both must be fully built and playable.

  How to use:
  - Set the paths to the games below in "CONFIGURATION."
  - Version 1.0.0 should be set to "OldGamePath".
  - The new build should be set to "NewGamePath".
  - Set the "GameVersion" which will output to that folder in "Resources".
  - Right click this script, select "Run with PowerShell".
  - Generated patches can be found in the "Resources" folder.
  - Obviously, the xdelta patches can be found in this folder.

  What to do with patches:
  - Open "LADXHD_Patcher.sln" in Visual Studio 2022.
  - In Solution Explorer, go to "Properties >> Resources.resx"
  - Double click "Resources.resx" to open it in a window.
  - Select all "xdelta3 patches" currently in Resources.resx and delete them.
  - Drag and drop all the new patches from the "patches" folder in "Resources.resx".
  - For easier identification and sorting later, set Neutral Comment to "xdelta3 patch".

  Now what?:
  - Edit the version number in "Program >> Config" to set the new version of the game.
  - Build the project. This will create a new patcher. All patches are handled automatically.

#>
#========================================================================================================================================
# CONFIGURATION
#========================================================================================================================================

$OldGamePath = "C:\Users\Bighead\source\repos\Zelda-LA-DX-HD_Stuff\original"
$NewGamePath = "C:\Users\Bighead\source\repos\Zelda-LA-DX-HD_Stuff\updated"
$GameVersion = "1.2.4"

#========================================================================================================================================
# SETUP XDELTA & OUTPUTS
#========================================================================================================================================

$BaseFolder  = Split-Path $script:MyInvocation.MyCommand.Path
$XDelta3     = Join-Path $BaseFolder "xdelta3.exe"
$PatchFolder = Join-Path $BaseFolder ("\Resources\v" + $GameVersion + " Patches")

#========================================================================================================================================
# MISCELLANEOUS
#========================================================================================================================================
$host.UI.RawUI.WindowTitle = "Link's Awakening DX HD: XDelta Patch Generation Script"

function PauseBeforeClose
{
    Write-Host "Press any key to close this window."
    [void][System.Console]::ReadKey()
    Exit
}

#========================================================================================================================================
# VERIFICATION
#========================================================================================================================================

if (!(Test-Path (Join-Path $OldGamePath "Link's Awakening DX HD.exe"))) {
    Write-Host "Invalid path for original game (OldGamePath)."
    PauseBeforeClose
}
if (!(Test-Path (Join-Path $NewGamePath "Link's Awakening DX HD.exe"))) {
    Write-Host "Invalid path for updated game (NewGamePath)."
    PauseBeforeClose
}
if (!(Test-Path $XDelta3)) {
    Write-Host "Missing xdelta3.exe in script folder."
    PauseBeforeClose
}

#========================================================================================================================================
# CREATE PATCHES FOLDER
#========================================================================================================================================

if (!(Test-Path $PatchFolder)) {
    New-Item -Path $PatchFolder -ItemType Directory | Out-Null
}

#========================================================================================================================================
# SPECIAL CASES
#========================================================================================================================================

$langFiles  = @("esp.lng", "fre.lng", "ita.lng", "por.lng", "rus.lng")
$langDialog = @("dialog_esp.lng", "dialog_fre.lng", "dialog_ita.lng", "dialog_por.lng", "dialog_rus.lng")
$smallFonts = @("smallFont_redux.xnb", "smallFont_vwf.xnb", "smallFont_vwf_redux.xnb")
$backGround = @("menuBackgroundB.xnb", "menuBackgroundC.xnb")
$npcImages  = @("npcs_redux.png")
$itemImages = @("items_esp.png", "items_redux.png", "items_redux_esp.png")
$introImage = @("intro_esp.png")
$miniMapImg = @("minimap_esp.png")
$objectsImg = @("objects_esp.png")
$photograph = @("photos_esp.png", "photos_redux.png", "photos_redux_esp.png")
$uiImages   = @("ui_esp.png")

$FileTargets = @{
    "eng.lng"             = $langFiles
    "dialog_eng.lng"      = $langDialog
    "smallFont.xnb"       = $smallFonts
    "menuBackground.xnb"  = $backGround
    "npcs.png"            = $npcImages
    "items.png"           = $itemImages
    "intro.png"           = $introImage
    "minimap.png"         = $miniMapImg
    "objects.png"         = $objectsImg
    "photos.png"          = $photograph
    "ui.png"              = $uiImages
}

function Build-ReverseMap($Targets)
{
    $Reverse = @{}

    foreach ($Key in $Targets.Keys) 
	{
        $ShortName = $Key
        $LongNames = $Targets[$Key]

        foreach ($LongName in $LongNames) 
		{
            $Reverse[$LongName.ToLower()] = $ShortName
        }
    }
    return $Reverse
}
$ReverseFileTargets = Build-ReverseMap -Targets $FileTargets

function GetOldFilePath([object]$File, [string]$RelativePath)
{
    if ($ReverseFileTargets.ContainsKey($File.Name.ToLower())) 
	{
		return Join-Path $OldGamePath ($File.DirectoryName.Substring($OldGamePath.Length).TrimStart('\') + "\" + $ReverseFileTargets[$File.Name.ToLower()] )
    }
	return Join-Path $OldGamePath $RelativePath
}

#========================================================================================================================================
# GENERATE PATCHES
#========================================================================================================================================

Write-Host ("Generating new patches for Link's Awakening DX HD v" + $GameVersion + "...")
Write-Host ""

foreach ($file in Get-ChildItem -LiteralPath $NewGamePath -Recurse -File) 
{
    $RelativePath = $file.FullName.Substring($NewGamePath.Length).TrimStart('\')
    $OldFilePath  = GetOldFilePath -File $file -RelativePath $RelativePath
    $NewFilePath  = $file.FullName

    if (!(Test-Path -LiteralPath $OldFilePath)) { continue }

    $OldMD5 = (Get-FileHash -Path $OldFilePath -Algorithm MD5).Hash
    $NewMD5 = (Get-FileHash -Path $NewFilePath -Algorithm MD5).Hash

    if ($OldMD5 -ne $NewMD5) 
    {
        $PatchFile = Join-Path $PatchFolder ($file.Name + ".xdelta")

        Write-Host ("Generating patch for: " + $file.Name)
        & $XDelta3 -f -e -s $OldFilePath $NewFilePath $PatchFile
    }
}
Write-Host ""
Write-Host "Patch generation complete. Patches can be found in folder:"
Write-Host $PatchFolder
Write-Host ""
PauseBeforeClose