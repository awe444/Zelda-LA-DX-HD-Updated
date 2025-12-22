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

**Solution:**
1. Ensure you have Visual C++ Redistributable 2015-2022 (x64) installed
2. Run `dotnet restore ProjectZ.csproj` to ensure WindowsDX package is properly restored
3. Check that the build is using the correct .NET SDK version (6.0+)
4. If the error persists, try cleaning the build:
   ```cmd
   dotnet clean ProjectZ.csproj
   dotnet restore ProjectZ.csproj
   dotnet build ProjectZ.csproj -r linux-arm64
   ```

The conditional package reference for `MonoGame.Framework.WindowsDX` provides the necessary DLLs for shader compilation on Windows.

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

### Issue: Black Screen on Launch

Try running with different fullscreen modes:
```bash
# Windowed mode (default)
./Link\'s\ Awakening\ DX\ HD

# Exclusive fullscreen
./Link\'s\ Awakening\ DX\ HD exclusive
```

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
