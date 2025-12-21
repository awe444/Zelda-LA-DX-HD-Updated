# Building on Windows 10/11 x64 - Minimal Environment Setup

This guide provides step-by-step instructions for building The Legend of Zelda: Link's Awakening DX HD game on a Windows 10/11 x64 system using **only command-line tools**, without requiring Visual Studio or any IDE installation.

**Note:** This guide assumes you have pre-migrated game assets in the build directory. The Patcher and Migrater tools do not need to be built in a minimal Windows environment.

## Table of Contents

- [Prerequisites](#prerequisites)
- [SDK Installation](#sdk-installation)
  - [.NET 6.0 SDK](#net-60-sdk)
- [Environment Verification](#environment-verification)
- [Building the Game](#building-the-game)
- [Build Configurations](#build-configurations)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

This guide assumes:
- A fresh Windows 10 x64 or Windows 11 x64 installation
- Administrator access to install software
- Internet connection for downloading dependencies
- Approximately 500 MB of free disk space (for .NET SDK and build outputs)
- Pre-migrated game assets already in the `ladxhd_game_source_code` directory

## SDK Installation

### .NET 6.0 SDK

The main game project (ProjectZ) requires the .NET 6.0 SDK.

**What it is:** The .NET 6.0 SDK includes the compiler, runtime, and tools needed to build .NET applications from the command line.

**Why it's needed:** The game is built using .NET 6.0 and MonoGame framework, which requires this SDK.

#### Installation Steps:

1. **Download the .NET 6.0 SDK:**
   - Visit the official Microsoft download page: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
   - Download the "SDK x64" installer (version 6.0.428 or later)
   - Click on the "SDK x64" link to download the latest installer
   - Note: The specific installer filename will vary based on version (e.g., `dotnet-sdk-6.0.xxx-win-x64.exe`)
   - Verify the download completed successfully (file size should be approximately 150 MB)

2. **Install the SDK:**
   - Run the downloaded installer (filename will be similar to `dotnet-sdk-6.0.xxx-win-x64.exe`)
   - If Windows shows a security warning, click "More info" → "Run anyway"
   - Follow the installation wizard (default options are fine)
   - The installer will add the `dotnet` command to your system PATH
   - If installation fails, see [Troubleshooting](#issue-net-sdk-installation-fails-with-error-0x80070570-or-0x80070642)

3. **Verify Installation:**
   ```batch
   dotnet --version
   ```
   Expected output: A version starting with `6.0` (e.g., `6.0.428` or later)

   ```batch
   dotnet --list-sdks
   ```
   Should show the installed .NET 6.0 SDK

---

## Environment Verification

After installing the .NET 6.0 SDK, verify your environment is correctly configured:

```batch
dotnet --version
dotnet --list-sdks
```

Expected output includes `6.0.xxx` in the SDK list.

---

## Building the Game

The game uses .NET 6.0 and can be built using the `dotnet` CLI.

#### Prerequisites:
- .NET 6.0 SDK installed
- Game assets must be pre-migrated and present in the build directory

#### Build Commands:

**Navigate to the game source directory:**
```batch
cd ladxhd_game_source_code
```

**Restore dependencies and tools:**
```batch
dotnet tool restore
dotnet restore
```

**Build for Debug:**
```batch
dotnet build -c Debug
```

**Build for Release:**
```batch
dotnet build -c Release
```

**Publish (create distributable package):**
```batch
dotnet publish -c Release -p:"PublishProfile=FolderProfile"
```

Or simply run the provided batch script:
```batch
publish.bat
```

**Output Location:**
- Debug build: `bin\Debug\net6.0-windows\`
- Release build: `bin\Release\net6.0-windows\`
- Published build: `Publish\` folder (created in the game source directory)

---

## Build Configurations

### Debug vs. Release

- **Debug:** Includes debugging symbols, no optimizations, larger binary size. Use for development and troubleshooting.
- **Release:** Optimized code, smaller binary size, suitable for distribution.

To build a specific configuration:

```batch
dotnet build -c Debug
dotnet build -c Release
```

---

## Troubleshooting

### Issue: .NET SDK installation fails with error 0x80070570 or 0x80070642

**Cause:** The downloaded installer file is corrupted or incomplete.

**Solution:**
1. Delete the downloaded installer file
2. Clear your browser's download cache
3. Re-download the .NET SDK installer from the official Microsoft website
4. Verify the download completed successfully (file size should be ~150MB)
5. If the problem persists:
   - Try downloading using a different browser
   - Temporarily disable antivirus software during download and installation
   - Download from a different network connection if possible
   - Use the offline installer instead of the web installer

**Alternative:** Use the .NET SDK installer via winget (Windows Package Manager):
```batch
winget install Microsoft.DotNet.SDK.6
```

---

### Issue: `dotnet` command not found

**Cause:** .NET SDK not installed or not in PATH.

**Solution:**
1. Verify .NET SDK is installed: Check "Add or Remove Programs" for ".NET SDK"
2. Restart your command prompt window
3. If still not working, log out and log back in to refresh environment variables
4. Reinstall .NET SDK if necessary

---

### Issue: "The command 'dotnet tool restore' exited with code 1"

**Cause:** The `dotnet-tools.json` file is blocked by Windows.

**Solution:**
1. Run the provided PowerShell script:
   ```powershell
   .\Unblock-All-Files.ps1
   ```
2. Or manually unblock the file:
   - Navigate to `ladxhd_game_source_code\.config\dotnet-tools.json`
   - Right-click → Properties → Check "Unblock" → OK

---

### Issue: Build fails with MonoGame content errors

**Cause:** MonoGame content builder tools not installed or not restored.

**Solution:**
1. Navigate to `ladxhd_game_source_code`
2. Run:
   ```batch
   dotnet tool restore
   ```
3. Ensure game assets are properly migrated and present in the build directory

---

### Issue: TextureImporter failures with PNG files

**Cause:** Content files are blocked by Windows or MonoGame content pipeline tools are not properly unblocked.

**Solution:**
1. Run the unblock script from the repository root:
   ```powershell
   .\Unblock-All-Files.ps1
   ```
2. If the script doesn't resolve the issue, manually unblock the MonoGame tools:
   - Navigate to `C:\Users\<YourUsername>\.nuget\packages\monogame.content.builder.task\3.8.1.303\tools`
   - Right-click each `.exe` and `.dll` file → Properties → Check "Unblock" → OK
   - Or run this PowerShell command from the repository root:
   ```powershell
   Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\monogame.content.builder.task" -Recurse -File | Unblock-File
   ```
3. Clean the build cache and retry:
   ```batch
   cd ladxhd_game_source_code
   dotnet clean
   dotnet tool restore
   dotnet restore
   dotnet build
   ```

---

### Issue: "Access denied" or permission errors during build

**Cause:** Files downloaded from the internet may be blocked by Windows.

**Solution:**
1. Run the unblock script:
   ```powershell
   .\Unblock-All-Files.ps1
   ```
2. Or manually unblock files by right-clicking → Properties → Unblock
3. If you're still seeing TextureImporter errors, see [TextureImporter failures](#issue-textureimporter-failures-with-png-files)

---

### Issue: PowerShell script execution is disabled

**Cause:** PowerShell execution policy prevents running scripts.

**Solution:**
Run PowerShell as Administrator and execute:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Then retry running the script.

---

## Additional Resources

- [.NET 6.0 Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)
- [MonoGame Documentation](https://docs.monogame.net/)

---

## Quick Reference: Complete Build Process

For building the game from scratch:

```batch
REM Navigate to repository root
cd C:\path\to\Zelda-LA-DX-HD-Updated

REM Unblock files (run in PowerShell)
powershell -ExecutionPolicy Bypass -File .\Unblock-All-Files.ps1

REM Build the game
cd ladxhd_game_source_code
dotnet tool restore
dotnet restore
dotnet publish -c Release -p:"PublishProfile=FolderProfile"
cd ..
```

Or simply run the provided helper script:
```batch
build-game.bat
```
