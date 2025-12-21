# Building on Windows 10/11 x64 - Minimal Environment Setup

This guide provides step-by-step instructions for building The Legend of Zelda: Link's Awakening DX HD and related tools on a Windows 10/11 x64 system using **only command-line tools**, without requiring Visual Studio or any IDE installation.

## Table of Contents

- [Prerequisites](#prerequisites)
- [SDK Installation](#sdk-installation)
  - [.NET 6.0 SDK](#net-60-sdk)
  - [Build Tools for Visual Studio 2022](#build-tools-for-visual-studio-2022)
- [Environment Verification](#environment-verification)
- [Building the Projects](#building-the-projects)
  - [Building the Game (ProjectZ)](#building-the-game-projectz)
  - [Building the Patcher](#building-the-patcher)
  - [Building the Migrater](#building-the-migrater)
- [Build Configurations](#build-configurations)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

This guide assumes:
- A fresh Windows 10 x64 or Windows 11 x64 installation
- Administrator access to install software
- Internet connection for downloading dependencies
- Approximately 5-10 GB of free disk space (for SDKs and build outputs)

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

2. **Install the SDK:**
   - Run the downloaded installer (filename will be similar to `dotnet-sdk-6.0.xxx-win-x64.exe`)
   - Follow the installation wizard (default options are fine)
   - The installer will add the `dotnet` command to your system PATH

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

### Build Tools for Visual Studio 2022

The Patcher and Migrater projects use the legacy .NET Framework 4.8.1, which requires MSBuild and the .NET Framework targeting pack.

**What it is:** Build Tools for Visual Studio provides MSBuild, compilers, and .NET Framework SDKs for building .NET Framework applications without installing the full Visual Studio IDE.

**Why it's needed:** The Patcher and Migrater projects are legacy .NET Framework 4.8.1 applications that require MSBuild to compile.

#### Installation Steps:

1. **Download Build Tools for Visual Studio 2022:**
   - Visit: https://visualstudio.microsoft.com/downloads/
   - Scroll down to "All Downloads" → expand "Tools for Visual Studio"
   - Download "Build Tools for Visual Studio 2022"
   - Direct link: https://aka.ms/vs/17/release/vs_buildtools.exe

2. **Install Required Components:**
   - Run the installer (`vs_buildtools.exe`)
   - In the installer, select the **Workloads** tab
   - Check **".NET desktop build tools"**
   - In the **Individual components** tab (on the right), ensure these are selected:
     - `.NET Framework 4.8.1 SDK`
     - `.NET Framework 4.8.1 targeting pack`
     - `MSBuild` (included with .NET desktop build tools)
   - Click **Install** (this will download ~2-4 GB)

3. **Installation Location:**
   - Default installation path: `C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\`
   - MSBuild location: `C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe`

4. **Add MSBuild to PATH (Optional but Recommended):**
   
   To use `msbuild` directly from any command prompt:
   
   - Open **Start Menu** → Search for "Environment Variables"
   - Click "Edit the system environment variables"
   - Click "Environment Variables..." button
   - Under "System variables", find and select "Path", then click "Edit..."
   - Click "New" and add: `C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin`
   - Click "OK" on all dialogs
   - **Close and reopen any command prompt windows** for changes to take effect

   Alternatively, use the **Developer Command Prompt** (see below).

---

## Environment Verification

After installing both SDKs, verify your environment is correctly configured:

### Verify .NET 6.0 SDK:

```batch
dotnet --version
dotnet --list-sdks
```

Expected output includes `6.0.xxx` in the SDK list.

### Verify MSBuild:

**Option 1: Using full path (always works):**
```batch
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" /version
```

**Option 2: Using PATH (if you added MSBuild to PATH):**
```batch
msbuild /version
```

**Option 3: Using Developer Command Prompt:**
- Open **Start Menu** → Search for "Developer Command Prompt for VS 2022"
- Run `msbuild /version`

Expected output: MSBuild version 17.x.x.x

### Verify .NET Framework Targeting Packs:

```batch
dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1"
```

Should show files without errors.

---

## Building the Projects

### Building the Game (ProjectZ)

The main game uses .NET 6.0 and can be built using the `dotnet` CLI.

#### Prerequisites:
- .NET 6.0 SDK installed
- Game assets must be set up (see README.md → "Updating Source Code Assets")

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

### Building the Patcher

The Patcher uses .NET Framework 4.8.1 and requires MSBuild.

#### Prerequisites:
- Build Tools for Visual Studio 2022 installed

#### Build Commands:

**Navigate to the patcher source directory:**
```batch
cd ladxhd_patcher_source_code
```

**Using Developer Command Prompt (Recommended):**

1. Open "Developer Command Prompt for VS 2022" from Start Menu
2. Navigate to the patcher directory:
   ```batch
   cd /d C:\path\to\Zelda-LA-DX-HD-Updated\ladxhd_patcher_source_code
   ```
3. Build the solution:
   ```batch
   msbuild LADXHD_Patcher.sln /p:Configuration=Debug
   msbuild LADXHD_Patcher.sln /p:Configuration=Release
   ```

**Using regular Command Prompt (with full MSBuild path):**
```batch
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" LADXHD_Patcher.sln /p:Configuration=Release
```

**Output Location:**
- `bin\Debug\LADXHD_Patcher.exe` (Debug build)
- `bin\Release\LADXHD_Patcher.exe` (Release build)

---

### Building the Migrater

The Migrater uses .NET Framework 4.8.1 and requires MSBuild.

#### Prerequisites:
- Build Tools for Visual Studio 2022 installed

#### Build Commands:

**Navigate to the migrater source directory:**
```batch
cd ladxhd_migrate_source_code
```

**Using Developer Command Prompt (Recommended):**

1. Open "Developer Command Prompt for VS 2022" from Start Menu
2. Navigate to the migrater directory:
   ```batch
   cd /d C:\path\to\Zelda-LA-DX-HD-Updated\ladxhd_migrate_source_code
   ```
3. Build the solution:
   ```batch
   msbuild LADXHD_Migrater.sln /p:Configuration=Debug
   msbuild LADXHD_Migrater.sln /p:Configuration=Release
   ```

**Using regular Command Prompt (with full MSBuild path):**
```batch
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" LADXHD_Migrater.sln /p:Configuration=Release
```

**Output Location:**
- `bin\Debug\LADXHD_Migrater.exe` (Debug build)
- `bin\Release\LADXHD_Migrater.exe` (Release build)

---

## Build Configurations

### Debug vs. Release

- **Debug:** Includes debugging symbols, no optimizations, larger binary size. Use for development and troubleshooting.
- **Release:** Optimized code, smaller binary size, suitable for distribution.

### Common MSBuild Switches

For .NET Framework projects (Patcher, Migrater):

```batch
msbuild <solution>.sln /p:Configuration=<Debug|Release> /p:Platform=AnyCPU /v:minimal
```

Options:
- `/p:Configuration=<Debug|Release>` - Build configuration
- `/p:Platform=AnyCPU` - Target platform
- `/v:minimal` - Set verbosity level (quiet, minimal, normal, detailed, diagnostic)
- `/t:Clean` - Clean build outputs before building
- `/t:Rebuild` - Clean and rebuild

Example (Clean and Rebuild):
```batch
msbuild LADXHD_Patcher.sln /t:Rebuild /p:Configuration=Release
```

---

## Troubleshooting

### Issue: `dotnet` command not found

**Cause:** .NET SDK not installed or not in PATH.

**Solution:**
1. Verify .NET SDK is installed: Check "Add or Remove Programs" for ".NET SDK"
2. Restart your command prompt window
3. If still not working, log out and log back in to refresh environment variables
4. Reinstall .NET SDK if necessary

---

### Issue: `msbuild` command not found

**Cause:** Build Tools not installed or MSBuild not in PATH.

**Solution:**
1. Use the full path to MSBuild: `"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"`
2. Or use "Developer Command Prompt for VS 2022" from Start Menu
3. Or add MSBuild to PATH (see installation steps above)

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

### Issue: "The reference assemblies for .NETFramework,Version=v4.8.1 were not found"

**Cause:** .NET Framework 4.8.1 targeting pack not installed.

**Solution:**
1. Run the Build Tools installer again (`vs_buildtools.exe`)
2. Click "Modify" on your installation
3. Ensure ".NET desktop build tools" workload is selected
4. In "Individual components", ensure `.NET Framework 4.8.1 SDK` and `.NET Framework 4.8.1 targeting pack` are checked
5. Click "Modify" to install missing components

---

### Issue: Build fails with MonoGame content errors

**Cause:** MonoGame content builder tools not installed or not restored.

**Solution:**
1. Navigate to `ladxhd_game_source_code`
2. Run:
   ```batch
   dotnet tool restore
   ```
3. Ensure game assets are properly set up (see README.md)

---

### Issue: "Access denied" or permission errors during build

**Cause:** Files downloaded from the internet may be blocked by Windows.

**Solution:**
1. Run the unblock script:
   ```powershell
   .\Unblock-All-Files.ps1
   ```
2. Or manually unblock files by right-clicking → Properties → Unblock

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
- [MSBuild Command-Line Reference](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-command-line-reference)
- [Build Tools for Visual Studio](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022)
- [MonoGame Documentation](https://docs.monogame.net/)

---

## Quick Reference: Complete Build Process

For a complete build of all projects from scratch:

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

REM Build the patcher (using Developer Command Prompt or full MSBuild path)
cd ladxhd_patcher_source_code
msbuild LADXHD_Patcher.sln /p:Configuration=Release
cd ..

REM Build the migrater (using Developer Command Prompt or full MSBuild path)
cd ladxhd_migrate_source_code
msbuild LADXHD_Migrater.sln /p:Configuration=Release
cd ..
```

Note: For MSBuild commands, use Developer Command Prompt or replace `msbuild` with the full path:
```batch
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
```
