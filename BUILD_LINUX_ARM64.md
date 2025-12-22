# Building for Ubuntu Linux ARM64

This guide provides instructions for building The Legend of Zelda: Link's Awakening DX HD game for Ubuntu Linux ARM64 deployment. The project supports **cross-compilation from Windows x64** as well as native builds on Linux ARM64.

## Table of Contents

- [Overview](#overview)
- [Cross-Compilation from Windows](#cross-compilation-from-windows)
- [Native Build on Linux ARM64](#native-build-on-linux-arm64)
- [Known Limitations](#known-limitations)
- [Troubleshooting](#troubleshooting)

---

## Overview

The game has been migrated from Windows x64 with DirectX to ARM64 Linux with OpenGL rendering. Key changes include:

- **Graphics API**: DirectX → OpenGL (via MonoGame DesktopGL)
- **Platform**: Windows x64 → Linux ARM64
- **Audio**: GbsPlayer background music is stubbed out (not available on this platform)
- **Build System**: Continues to use command-line `dotnet` tools

## Cross-Compilation from Windows

You can build the Linux ARM64 binary on Windows x64 systems. This is useful for developers working on Windows who want to deploy to Linux ARM64.

### Prerequisites (Windows)

- Windows 10/11 x64
- .NET 6.0 SDK or later
- Visual C++ Redistributable 2015-2022 (x64)

### Installation Steps (Windows)

1. **Install .NET 6.0 SDK:**
   - Download from: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
   - Install the SDK x64 installer

2. **Verify installation:**
   ```cmd
   dotnet --version
   ```

### Building on Windows for Linux ARM64

1. **Navigate to project directory:**
   ```cmd
   cd Zelda-LA-DX-HD-Updated\ladxhd_game_source_code
   ```

2. **Restore dependencies:**
   ```cmd
   dotnet restore ProjectZ.csproj
   ```

3. **Build for Linux ARM64:**
   ```cmd
   dotnet build ProjectZ.csproj -r linux-arm64 -c Release
   ```

4. **Publish for deployment:**
   ```cmd
   dotnet publish ProjectZ.csproj -r linux-arm64 -c Release
   ```

The published game will be located at:
```
bin\Release\net6.0\linux-arm64\publish\
```

**Important**: The resulting binary is for Linux ARM64 and **will not run on Windows**. Transfer the entire `publish` folder to your Linux ARM64 system to run the game.

## Native Build on Linux ARM64

## Native Build on Linux ARM64

If you prefer to build directly on your Linux ARM64 target system, follow these instructions.

### Prerequisites (Linux)

- Ubuntu Linux ARM64 system (tested on Ubuntu 20.04+)
- Root or sudo access to install software
- Internet connection for downloading dependencies
- Approximately 500 MB of free disk space
- Pre-migrated game assets already in the `ladxhd_game_source_code/Data` directory

### SDK Installation (Linux)

### SDK Installation (Linux)

#### .NET 6.0 SDK

The game project requires the .NET 6.0 SDK.

###### Installation Steps:

1. **Add Microsoft package repository:**
   ```bash
   wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   rm packages-microsoft-prod.deb
   ```

2. **Install .NET 6.0 SDK:**
   ```bash
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-6.0
   ```

3. **Verify installation:**
   ```bash
   dotnet --version
   ```
   You should see version 6.0.xxx

#### Required System Libraries

MonoGame on Linux requires several system libraries:

```bash
sudo apt-get install -y \
    libsdl2-dev \
    libopenal-dev \
    libfreetype6-dev \
    libgdiplus
```

### Building the Game (Linux)

##### Navigate to Project Directory

```bash
cd Zelda-LA-DX-HD-Updated/ladxhd_game_source_code
```

##### Restore Dependencies

```bash
dotnet restore ProjectZ.csproj
```

##### Build the Game

For a debug build:
```bash
dotnet build ProjectZ.csproj -r linux-arm64
```

For a release build:
```bash
dotnet build ProjectZ.csproj -r linux-arm64 -c Release
```

#### Publish the Game

To create a self-contained executable:
```bash
dotnet publish ProjectZ.csproj -r linux-arm64 -c Release
```

The published game will be located at:
```
bin/Release/net6.0/linux-arm64/publish/
```

#### Run the Game

```bash
cd bin/Release/net6.0/linux-arm64/publish/
./Link\'s\ Awakening\ DX\ HD
```

Or with command-line arguments:
```bash
# Enable exclusive fullscreen
./Link\'s\ Awakening\ DX\ HD exclusive

# Load a specific save slot
./Link\'s\ Awakening\ DX\ HD loadSave 1

# Enable editor mode (if you have a keyboard)
./Link\'s\ Awakening\ DX\ HD editor
```

## How Cross-Compilation Works

The project uses a conditional package reference strategy to support building on Windows for Linux ARM64:

1. **MonoGame.Framework.DesktopGL** - The main runtime package for OpenGL on Linux
2. **MonoGame.Framework.WindowsDX** (Windows only) - Provides Content Pipeline dependencies for Windows builds

When building on Windows, the project includes `MonoGame.Framework.WindowsDX` with `PrivateAssets=contentfiles;build`, which provides:
- `libmojoshader_64.dll` - For HLSL shader compilation
- Windows-specific Content Pipeline tools

These dependencies are **only used during the build process** on Windows and are not included in the final Linux ARM64 binary. The runtime binary only uses DesktopGL and OpenGL libraries.

**Key Points:**
- The Windows DLL dependencies are build-time only, not runtime dependencies
- The final Linux binary contains no Windows-specific code or libraries
- Shaders are compiled during the build phase and embedded as platform-independent bytecode
- The same `Data` folder works on both Windows (for building) and Linux (for running)

## Known Limitations

### Background Music (GbsPlayer) Not Available

The GbsPlayer audio system, which provides authentic Game Boy Sound music playback, relies on Windows-specific audio libraries (SharpDX/XAudio2) and is **not available** on Linux ARM64.

**What this means:**
- Background music will not play
- Sound effects work normally
- The game will display console warnings about stubbed audio on startup
- All other gameplay functionality is unaffected

**Console output you'll see:**
```
[GbsPlayer-STUB] GbsPlayer initialized in stub mode for ARM64 Linux. Background music disabled.
[GbsPlayer-STUB] LoadFile called but stubbed: Data/Music/awakening.gbs
```

### Graphics

The game uses OpenGL rendering via MonoGame's DesktopGL framework. Your system must support:
- OpenGL 3.0 or higher
- SDL2 for window management

### Window Resizing

The game supports window resizing. However, older versions of SDL bundled with MonoGame may not support changing the resizable parameter after window creation. The game handles this gracefully:

- If your SDL version supports it, the window will be resizable
- If not, the game will display a warning and run in fixed-size window mode
- This does not affect gameplay or other functionality

**Expected console output if window resizing is not supported:**
```
[WARNING] Could not enable window resizing: SDL 2.0.4 does not support changing resizable parameter...
The game will run in fixed-size window mode.
```

## Troubleshooting

### Issue: Missing SDL2 Libraries

**Error:**
```
error while loading shared libraries: libSDL2-2.0.so.0
```

**Solution:**
```bash
sudo apt-get install libsdl2-2.0-0 libsdl2-dev
```

### Issue: Missing OpenAL Libraries

**Error:**
```
error while loading shared libraries: libopenal.so.1
```

**Solution:**
```bash
sudo apt-get install libopenal1 libopenal-dev
```

### Issue: Build Warnings About .NET 6.0

**Warning:**
```
warning NETSDK1138: The target framework 'net6.0' is out of support
```

**Note:** This is informational only. The game builds successfully and runs on .NET 6.0. Future updates may migrate to newer .NET versions.

### Issue: Shader Compilation Errors on Windows (Cross-Compilation)

**Error:**
```
Unable to load DLL 'libmojoshader_64.dll' or one of its dependencies
```

**Root Cause:** The libmojoshader_64.dll has dependencies that may not be present or accessible on your system.

**Solutions to try (in order):**

1. **Install additional Visual C++ Redistributables:**
   - Install Visual C++ Redistributable 2013 (x64) - libmojoshader may depend on older runtime
   - Download from: https://aka.ms/highdpimfc2013x64enu
   - Also ensure Visual C++ Redistributable 2015-2022 (x64) is installed

2. **Check Windows SDK components:**
   - Install Windows 10/11 SDK if not already present
   - The shader compiler (D3DCompiler) dependencies may be needed

3. **Clean and rebuild:**
   ```cmd
   dotnet clean ProjectZ.csproj
   rd /s /q bin obj
   dotnet restore ProjectZ.csproj
   dotnet build ProjectZ.csproj -r linux-arm64 -c Release
   ```

4. **Verify package restoration:**
   - Check that WindowsDX package is restored: look for `.nuget\packages\monogame.framework.windowsdx\3.8.1.303`
   - Verify libmojoshader_64.dll exists in: `.nuget\packages\monogame.framework.windowsdx\3.8.1.303\runtimes\win-x64\native\`

5. **Use Dependency Walker or Dependencies.exe:**
   - Download Dependencies.exe from https://github.com/lucasg/Dependencies
   - Open libmojoshader_64.dll to see which DLL dependencies are missing
   - Install the missing dependencies

6. **Alternative: Pre-compile shaders on a working system:**
   - If cross-compilation continues to fail, compile shaders on a Linux system or a Windows system where it works
   - Copy the compiled Content folder to your Windows build machine
   - Disable Content Pipeline by removing Content.mgcb references

**Additional Notes:**
- The error "or one of its dependencies" usually means a transitive dependency is missing, not libmojoshader_64.dll itself
- Common missing dependencies: MSVCR120.dll, MSVCP120.dll (from VS2013), d3dcompiler_47.dll
- libmojoshader is a native C library that bridges HLSL shader compilation

### Issue: Permission Denied When Running

**Error:**
```
Permission denied
```

**Solution:**
```bash
chmod +x "./Link's Awakening DX HD"
```

### Issue: Content/Data Not Found

**Error:**
```
Could not find file 'Data/...'
```

**Solution:** Ensure the `Data` directory from the original game is present in the same directory as the executable. The published output should include it automatically via the `<Content Include="Data\**">` directive in the .csproj file.

### Issue: Display/Rendering Problems

If you experience graphics issues:
1. Update your graphics drivers
2. Check OpenGL version: `glxinfo | grep "OpenGL version"`
3. Ensure you have at least OpenGL 3.0 support

### Issue: SDL Window Resizing Error

**Error:**
```
Fatal error: SDL 2.0.4 does not support changing resizable parameter of the window after it's already been created
```

**Cause:** MonoGame 3.8.1.303 may bundle an older version of SDL2 that doesn't support runtime changes to window properties, even if you have a newer SDL2 installed on your system.

**Solution:** This has been fixed in the latest version. The game now handles this error gracefully and will run in fixed-size window mode if the SDL version doesn't support resizing. 

If you're running an older build:
1. Rebuild from the latest source code
2. The game will display a warning but continue to run normally
3. Window resizing will be disabled, but all other functionality works

**Note:** The game uses the SDL2 library bundled with MonoGame.Framework.DesktopGL, not your system's SDL2 installation.

### Issue: Black Screen on Launch

Try running with different fullscreen modes:
```bash
# Windowed mode (default)
./Link\'s\ Awakening\ DX\ HD

# Exclusive fullscreen
./Link\'s\ Awakening\ DX\ HD exclusive
```

### Issue: Game Crashes with NullReferenceException

**Error:**
```
Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
   at ProjectZ.InGame.GameObjects.Enemies.EnemySpinyBeetle..ctor(...)
```

**Cause:** This usually indicates missing game data files (Data folder) or file path case sensitivity issues.

**Solution:**
1. **Ensure Data folder is present:**
   ```bash
   ls -la ./Data/Animations/
   ```
   
2. **Check for case sensitivity issues:**
   The game will now log detailed error messages to help diagnose missing files:
   ```
   [ERROR] Animator file not found: Data/Animations/Enemies/spiny beetle.ani
   [ERROR] Found X .ani files in Data/Animations
   ```

3. **Verify all game data files are copied:**
   - Ensure the entire `Data` folder from the original game is present
   - The publish process should include it automatically via the `.csproj` configuration
   - If manually copying, preserve the directory structure exactly

4. **Check file permissions:**
   ```bash
   chmod -R 755 ./Data
   ```

5. **Rebuild with verbose logging:**
   The latest version includes enhanced error logging that will show which specific files are missing when the game crashes.

**Note about Windows paths in stack traces:** The paths shown in error messages (e.g., `C:\Users\...`) are from debug symbols generated during compilation on Windows. This is normal and doesn't indicate a problem with the Linux build itself.

## Differences from Windows Build

| Feature | Windows x64 | Linux ARM64 |
|---------|-------------|-------------|
| Graphics API | DirectX 11 | OpenGL 3.x+ |
| Background Music | ✅ Full GBS playback | ❌ Stubbed (no music) |
| Sound Effects | ✅ | ✅ |
| Windowed Mode | ✅ | ✅ |
| Borderless Fullscreen | ✅ | ⚠️ Uses standard fullscreen |
| Exclusive Fullscreen | ✅ | ✅ |
| Editor Mode | ✅ | ✅ |
| Controller Support | ✅ | ✅ (via SDL2) |

## Build Configuration Details

The project has been configured with:
- **Target Framework**: net6.0 (cross-platform)
- **Runtime Identifier**: linux-arm64
- **MonoGame Platform**: DesktopGL
- **Output Type**: Exe (console application)
- **Self-Contained**: Optional (via publish)

## Migration Summary

This build represents a complete platform migration:
1. ✅ Project file updated for linux-arm64 target
2. ✅ MonoGame switched from WindowsDX to DesktopGL
3. ✅ All Windows Forms dependencies removed
4. ✅ GbsPlayer audio system stubbed out
5. ✅ File path handling made cross-platform
6. ✅ Shader pipeline uses MonoGame's automatic HLSL→GLSL conversion
7. ✅ All Windows-specific code removed or replaced

---

For questions or issues specific to the Linux ARM64 build, please open an issue on GitHub with the `linux-arm64` label.
