# The Legend of Zelda: Links Awakening DX HD - PC Port

### $${\color{red}THIS \space REPOSITORY \space DOES \space NOT \space INCLUDE \space COPYRIGHTED \space GAME \space ASSETS!}$$

To use this fork, whether to play the game or build upon, it requires the user to provide the assets from the original v1.0.0 release.   
Some assets have been updated, but I have created tooling to make migration easier.

- This is a continuation of my [previous fork](https://github.com/BigheadSMZ/Links-Awakening-DX-HD) and here's a link to the [commits](https://github.com/BigheadSMZ/Links-Awakening-DX-HD/commits/master/).
- See the [manual](https://github.com/BigheadSMZ/Zelda-LA-DX-HD-Updated/blob/main/MANUAL.md) to learn more about the game (WIP).
- See the [changelog](https://github.com/BigheadSMZ/Zelda-LA-DX-HD-Updated/blob/main/CHANGELOG.md) for a list of changes from v1.0.0.
- As of v1.1.0, the game is in a really good state and the "feel" is really close to the original game.  
- As of v1.2.0, the vast majority of bugs have been fixed and features from the [Redux romhack](https://github.com/ShadowOne333/Links-Awakening-Redux) were implemented.
- As of v1.3.0, I consider the work that I've done to be "feature complete" and everything from this point is gravy.
- As of v1.4.0, the gravy train never stopped and so much has been done to make this port more accurate to the original games.

## Patching v1.0.0 (or v1.1.4+) to v1.4.9.

To download the latest update, there is a patcher on the [Releases](https://github.com/BigheadSMZ/Zelda-LA-DX-HD-Updated/releases) page. 
If you wish to build the game yourself, see **Personal Build / Publishing**.
- Find the v1.0.0 release originally from itch.io.
- If you can not find it, you can search for an "archive" of it.
- It's a good idea to keep a <ins>backup</ins> of v1.0.0.
- Download the patcher from the releases page.
- Drop it into the same folder as v1.0.0/v1.1.4+.
- Open the patcher. Press the "Patch" button.
- When it is done, the patcher can be deleted.

## About This Repository

A few years back, an anonymous user posted a PC Port of Link's Awakening on itch.io built with MonoGame. It wasn't long before the game was taken down, fortunately the release contained the source code. This is a continuation of that PC Port but with the assets stripped away to avoid copyright issues. 

This section explains the files and folders found in the base of this respository.
- **assets_original**: This is where the **"Content"** and **"Data"** folders from v1.0.0 should go.
- **assets_patches**: Contains xdelta3 patches that are the difference of assets from v1.0.0 to the latest updates.
- **ladxhd_game_source_code**: Source code for The Legend of Zelda: Link's Awakening DX HD.
- **ladxhd_migrate_source_code**: Source code for the migration tool which can apply/create assets patches.
- **ladxhd_patcher_source_code**: Source code for the patcher to update the game to v1.4.9.
- **LADXHD_Migrater.exe**: This is the migration tool used to apply or create patches to the assets.
- **Unblock-All-Files.ps1**: This script can be used to unblock all files automatically for Visual Studio.

## Updating Source Code Assets

The latest source code can be downloaded from this repository. But, you will need to provide the assets from the original v1.0.0 release. It is very important to follow the instructions carefully as many assets have been updated.
- You will notice there is a folder in the base of this repository named **"assets_original"**.
- This is where the **"Content"** and **"Data"** folders go from the v1.0.0 release.
- Note that there is two versions of these folders, and you must provide the correct ones.
- Inside the original release folder are two folders: **"Content"** and **"Data"**.
- Copy the **"Data"** folder from the original v1.0.0 <ins>game folder</ins> to the **"assets_original"** folder.
- This is NOT the correct **"Content"** folder. You need the one from the source code.
- There should also be a 7-Zip of the v1.0.0 source code included with the game: **"source.7z"**.
- Unzip the **"source.7z"** file from the original v1.0.0 release.
- Copy the **"Content"** folder from the original v1.0.0 <ins>source code folder</ins> to the **"assets_original"** folder.
- After both folders are copied, open the **"LADXHD_Migrater.exe"** tool that is provided.
- Click the button **"Migrate Assets From v1.0.0"** and wait for it to finish.
- This will create new **"Content"** and **"Data"** folders in the **"ladxhd_game_source_code"** folder.
- And you are done. From here you can build the game or work on the code.
- The original **"Content/Data"** folders should be kept in **"assets_original"** for future patches.

Again, make sure you are grabbing the correct Content and Data folders. The "Data" folder should come from the <ins>game folder</ins>, and the "Content" folder should come from the <ins>source .7z file</ins>. While it is possible the original assets would work, there have been bugs fixed and issues addressed in some of them. The patches in **"assets_patches"** never need to be interacted with directly, as the migration tool can handle both directions: updating 1.0.0 assets, and creating new patches for asset updates.

## Contributing Prerequisites

If you wish to work on the code in this repository.
- Basic knowledge of C# .NET and Visual Studio is required.
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
    - Make sure to select `.NET desktop development` components in the visual studio installer.
- The "base" game should remain as close to the original DX version as possible.
- Modifications are okay in the form of options, but should default to **<ins>false</ins>**.
- Some exceptions may be okay depending on their intent.

## Contributing Assets

Do not make pull requests providing assets directly. This includes ALL files within the **Content** and **Data** folders. Instead, use the **LADXHD Migrater** tool provided to create xdelta patches. These patches can then be applied to the original assets to update them to the latest versions also using the migration tool.

See the wiki page on [contributing to this project](https://github.com/BigheadSMZ/Zelda-LA-DX-HD-Updated/wiki/Contributing-to-this-project) for more information.

## Build Instructions

If you wish to build the code in this repository.
- Clone or Download this repository: green `Code` Button > `Download ZIP`
- The game's source code is in **"ladxhd_game_source_code"** folder
- Follow the steps in **Updating Source Code Assets**
- Run the PowerShell script "Unblock-All-Files.ps1".
  - -OR- Go to the folder `ladxhd_game_source_code\.config` you will see `dotnet-tools.json`.
  - -AND- Right click, go to properties, check `Unblock`.
- Open ProjectZ.sln
- Build/run like any normal C# program

## Personal Build / Publishing

To create a personal build, follow the steps below:
- Download and install [.NET v6.0.428 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.428-windows-x64-installer).
- Clone or Download this repository: green `Code` Button > `Download ZIP`
- Unzip the repository and open up the unzipped folder.
- Follow the steps in **Updating Source Code Assets**
- Run the PowerShell script "Unblock-All-Files.ps1".
  - -OR- Go to the folder `ladxhd_game_source_code\.config` you will see `dotnet-tools.json`.
  - -AND- Right click, go to properties, check `Unblock`.
- Run the `ladxhd_game_source_code\publish.bat` script to build the game.
- Alternatively, the **"LADXHD_Migrater.exe"** tool can now build the game.
- When done, the build will be in the `Publish` folder.

## Build Troubleshooting

If you experience the error **The command “dotnet tool restore” exited with code 1** then make sure the file **.config\dotnet-tools.json** isn't blocked. 

- To unblock all files in one go, run the included PowerShell script **"Unblock-All-Files.ps1"**.
- To unblock a single file: Right click, go to Properties, check Unblock, and click OK.

## Linux Build Instructions (Ubuntu)

### Prerequisites

Install the required dependencies on Ubuntu:

```bash
# Update package list
sudo apt-get update

# Install .NET 8 SDK
sudo apt-get install -y dotnet-sdk-8.0

# Install MonoGame DesktopGL runtime dependencies
# Note: SDL2 version 2.0.5 or newer is required
sudo apt-get install -y \
    libopenal-dev \
    libsdl2-dev \
    libgl1-mesa-dev \
    libglu1-mesa-dev \
    mesa-common-dev

# Install MonoGame Content Pipeline build dependencies
sudo apt-get install -y \
    libfreeimage3 \
    libfreeimage-dev

# Optional: Install Wine for asset migration (LADXHD_Migrater.exe is Windows-only)
# Wine is NOT needed for shader compilation - MonoGame's mgfxc tool handles that
sudo apt-get install -y wine64
```

### Setting Up Assets on Linux

**IMPORTANT: Shader Compilation on Linux**

Good news! **Wine is NOT required for shader compilation.** MonoGame includes a cross-platform shader compiler (`mgfxc`) that runs natively via .NET. The build process automatically compiles shaders for the DesktopGL (OpenGL) platform.

**Wine is only needed to run `LADXHD_Migrater.exe`** (the asset migration tool). You have two options:

**Option 1: Migrate Assets with Wine on Linux (Simplest)**

```bash
# Install Wine (64-bit) - only needed for the migrator tool
sudo apt-get install -y wine64

# Run the migrator tool (creates Content and Data folders)
wine LADXHD_Migrater.exe
# Click "Migrate Assets From v1.0.0" and wait for completion

# That's it! Shaders will compile automatically during build via mgfxc
```

**Option 2: Migrate Assets on Windows, Copy to Linux**

1. On Windows machine, follow the "Updating Source Code Assets" section
2. After running `LADXHD_Migrater.exe`, copy these folders to Linux:
   - `ladxhd_game_source_code/Content/` (source .fx files, images, fonts, etc.)
   - `ladxhd_game_source_code/Data/` (game maps, sprites, sounds, etc.)
3. Place both folders in `ladxhd_game_source_code` on Linux
4. Shaders will compile automatically during build via mgfxc

**Option 3: Copy from Existing Build (Pre-compiled)**

If you already have a DesktopGL build (from Linux or Windows with DesktopGL platform):
- Copy both `Content/` and `Data/` folders to `ladxhd_game_source_code` on Linux
- **WARNING:** Do NOT use Content from Windows DirectX (WindowsDX) builds - only DesktopGL builds are compatible

### Building on Linux

```bash
cd ladxhd_game_source_code

# Disable editor fonts (required for gameplay-only builds)
chmod +x disable_editor_fonts.sh
./disable_editor_fonts.sh

# Make the build script executable (first time only)
chmod +x publish_linux.sh

# Run the build script
# Shaders will be compiled automatically for OpenGL by mgfxc
./publish_linux.sh

# The executable will be in Publish/linux-arm64/
```

**How Shader Compilation Works:**

The build process uses MonoGame's `mgfxc` tool (installed via `dotnet tool restore`) to automatically compile `.fx` shader files to `.xnb` format for the OpenGL platform. This happens during the build via `MonoGame.Content.Builder.Task`. **No Wine required!**

**Alternative: Manual Build Command**

For ARM64 (Ubuntu 25.04 aarch64):
```bash
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true
```

### Running on Linux

```bash
cd Publish/linux-arm64
./"Link's Awakening DX HD"
```

### Linux-Specific Notes

- **Editor mode is not supported on Linux builds.** The build configuration skips editor fonts and UI components that require Windows Forms.
- All **gameplay features** work identically on Linux and Windows.
- Save files are stored in `~/.local/share/Zelda_LA/SaveFiles/` (or in the game directory if `portable.txt` exists).
- Settings are stored in `~/.local/share/Zelda_LA/settings` (or in the game directory if `portable.txt` exists).

### Linux Build Troubleshooting

**Error: "SDL 2.0.4 does not support changing resizable parameter"**

This error indicates your system has an older version of SDL2. Ubuntu 25.04 should have SDL2 2.0.5+ by default, but if you encounter this error:

```bash
# Check your SDL2 version
dpkg -l | grep libsdl2

# If version is < 2.0.5, upgrade SDL2
sudo apt-get update
sudo apt-get install --only-upgrade libsdl2-2.0-0 libsdl2-dev

# Verify the upgrade
dpkg -l | grep libsdl2
```

**FreeImage Library Not Found**

If you see "Unable to load shared library 'FreeImage'", create a symlink:

```bash
# Find the library
FREEIMAGE_PATH=$(find /usr/lib -name "libfreeimage.so.3" 2>/dev/null | head -1)

# Create symlink
sudo ln -sf "$FREEIMAGE_PATH" "$(dirname "$FREEIMAGE_PATH")/libFreeImage.so"

# Update library cache
sudo ldconfig
```

**Shader Compilation Details (Technical)**

MonoGame's shader compilation on Linux works through these components:

1. **mgfxc**: Cross-platform .NET tool that compiles HLSL shaders
   - Installed automatically via `dotnet tool restore` (see `.config/dotnet-tools.json`)
   - Runs natively on Linux (no Wine needed)
   - Compiles shaders for OpenGL profile when MonoGamePlatform=DesktopGL

2. **MonoGame.Content.Builder.Task**: MSBuild task in ProjectZ.csproj
   - Automatically invokes mgfxc during build
   - Processes all .fx files referenced in Content.mgcb
   - Outputs .xnb compiled shaders to Content folder

**If you encounter "Shader compilation failed" errors:**

```bash
# Verify mgfxc is installed
dotnet tool list

# Should show: dotnet-mgfxc    3.8.1.303    dotnet-mgfxc

# If missing, restore tools
cd ladxhd_game_source_code
dotnet tool restore
```

**CRITICAL:** MonoGame shaders are platform-specific. The DesktopGL platform compiles shaders for OpenGL. Do NOT use .xnb files from WindowsDX (DirectX) builds - they are incompatible and will cause "built for a different platform" runtime errors.

### Disabling Editor Fonts for Gameplay-Only Builds

**Linux builds skip editor mode and focus on gameplay only.** The Content Pipeline will fail if it tries to build editor fonts that require Windows fonts (Segoe UI, Courier New).

**Recommended: Use the provided script to disable editor fonts**

After migrating assets, run:
```bash
cd ladxhd_game_source_code
chmod +x disable_editor_fonts.sh
./disable_editor_fonts.sh
```

This script automatically comments out editor font references in `Content/Content.mgcb`, allowing the build to succeed without Windows fonts. The game will work perfectly for gameplay - only editor mode features will be unavailable.

**Manual Alternative:**
Edit `Content/Content.mgcb` and add `#` at the start of these lines and their associated properties:
```
#begin Content/Fonts/editor font.spritefont
#begin Content/Fonts/editor mono font.spritefont
#begin Content/Fonts/editor small mono font.spritefont
```

**Optional: Install Windows Fonts (Advanced Users Only)**
If you need editor mode support, you can install MS Core Fonts:
```bash
sudo apt-get install -y ttf-mscorefonts-installer
sudo fc-cache -f -v
```

## About This Fork

I am a terrible programmer, but I have a love for this game. A ton of forks popped up, some with fixes, but nowhere were they all centralized. This fork attempted to find and implement all the various fixes and improvements spread across the other various forks. Once that was done, I started tackling the issues from the repository this was cloned from. And after that was done, I worked on anything else I could find that would make the game feel more like the original game.

Feel free to commit any potential fixes as a PR. There are no coding guidelines and any style is welcome as long as the code either fixes something broken or makes the game behave closer to the original. But do try to at least keep it neat.
