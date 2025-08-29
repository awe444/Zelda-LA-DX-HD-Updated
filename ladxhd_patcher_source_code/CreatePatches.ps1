#========================================================================================================================================
# INSTRUCTIONS
#========================================================================================================================================
<#
  Info & Purpose:
  - Generate xdelta patches to update v1.0.0 to the latest build.
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
  - Right click this script, select "Run with PowerShell".
  - Generated patches can be found in the "Resources" folder.
  - The folder in "Resources" uses the "$GameVersion" set below.
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
$GameVersion = "1.1.6"

#========================================================================================================================================
# SETUP XDELTA & OUTPUTS
#========================================================================================================================================

$BaseFolder  = Split-Path $script:MyInvocation.MyCommand.Path
$XDelta3     = Join-Path $BaseFolder "xdelta3.exe"
$PatchFolder = Join-Path $BaseFolder ("\Resources\v" + $GameVersion + " Patches")

#========================================================================================================================================
# MISCELLANEOUS
#========================================================================================================================================
$host.UI.RawUI.WindowTitle = "Link's Awakening DX HD XDelta Patch Generation Script"

function PauseBeforeClose
{
    Write-Host ""
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
# CREATE LANGUAGE PATCHES FROM ENGLISH FILES
#========================================================================================================================================
$LanguageFiles  = @("esp","fre","ita","por","rus");
$LanguageDialog = @("dialog_esp","dialog_fre","dialog_ita","dialog_por","dialog_rus");

function CheckCreateLanguageFiles([object]$file)
{
    if (($file.Extension -eq ".lng") -and ($file.Name -notlike "*eng.lng"))
    {
        $RelativePath = $file.DirectoryName.Substring($OldGamePath.Length).TrimStart('\')

        if ($LanguageFiles.Contains($file.BaseName))
        {
            $EngPath = Join-Path $OldGamePath ($RelativePath + "\eng.lng")
        }
        elseif ($LanguageDialog.Contains($file.BaseName))
        {
            $EngPath = Join-Path $OldGamePath ($RelativePath + "\dialog_eng.lng")
        }
        $PatchFile = Join-Path $PatchFolder ($file.Name + ".xdelta")

        Write-Host ("Generating patch for: " + $file.Name)
        & $XDelta3 -f -e -s $EngPath $file.FullName $PatchFile
        
        return $true
    }
    return $false
}

#========================================================================================================================================
# GENERATE PATCHES
#========================================================================================================================================

Write-Host ("Generating new patches for Link's Awakening DX HD v" + $GameVersion + "...")
Write-Host ""

foreach ($file in Get-ChildItem -LiteralPath $NewGamePath -Recurse -File) 
{
    $RelativePath = $file.FullName.Substring($NewGamePath.Length).TrimStart('\')
    $OldFilePath  = Join-Path $OldGamePath $RelativePath
    $NewFilePath  = $file.FullName

    if (CheckCreateLanguageFiles($file)) { continue };
    if (!(Test-Path $OldFilePath)) { continue }

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