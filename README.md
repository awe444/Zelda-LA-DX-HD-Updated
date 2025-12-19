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

This project has been configured to build for **Linux ARM64** with **OpenGL** rendering backend.

### Prerequisites

#### For building from Windows using Visual Studio 2022:
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
  - Select `.NET desktop development` workload in the Visual Studio installer
  - Optional: Linux development tools (for advanced scenarios)
- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or higher
  - The project targets .NET 6.0 (cross-platform)
  - Cross-compilation for Linux ARM64 is built into the .NET SDK

#### For the target Linux ARM64 system:
- Linux ARM64 operating system (e.g., Raspberry Pi OS 64-bit, Ubuntu ARM64, etc.)
- OpenGL support (Mesa drivers or hardware OpenGL)
- Required dependencies:
  - `libgdiplus` (for System.Drawing functionality)
  - OpenGL libraries (`libgl1-mesa-glx` or equivalent)
  - FreeImage and other MonoGame dependencies

### Building the Project

If you wish to build the code in this repository.
- Clone or Download this repository: green `Code` Button > `Download ZIP`
- The game's source code is in **"ladxhd_game_source_code"** folder
- Follow the steps in **Updating Source Code Assets**
- Run the PowerShell script "Unblock-All-Files.ps1".
  - -OR- Go to the folder `ladxhd_game_source_code\.config` you will see `dotnet-tools.json`.
  - -AND- Right click, go to properties, check `Unblock`.
- Open ProjectZ.sln in Visual Studio 2022
- Build/run like any normal C# program (Note: Will build for Linux ARM64)

## Personal Build / Publishing

To create a personal build for Linux ARM64, follow the steps below:
- Download and install [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or higher
- Clone or Download this repository: green `Code` Button > `Download ZIP`
- Unzip the repository and open up the unzipped folder.
- Follow the steps in **Updating Source Code Assets**
- Run the PowerShell script "Unblock-All-Files.ps1" (Windows only)
  - -OR- Go to the folder `ladxhd_game_source_code\.config` you will see `dotnet-tools.json`.
  - -AND- Right click, go to properties, check `Unblock`.

### Publishing via Command Line:
```bash
cd ladxhd_game_source_code
dotnet publish -c Release -r linux-arm64 --self-contained true
```

The built files will be in: `bin/Release/net6.0/linux-arm64/publish/`

### Publishing via Publish Script:
- Run the `ladxhd_game_source_code\publish.bat` script (or use the command above)
- When done, the build will be in the publish output directory

### Publishing via Visual Studio 2022:
1. Open ProjectZ.sln in Visual Studio 2022
2. Right-click on the ProjectZ project in Solution Explorer
3. Select "Publish"
4. Use the FolderProfile publish profile
5. Click "Publish"

### Deploying to Linux ARM64:
1. Copy the entire publish output directory to your Linux ARM64 system
2. Make the executable runnable: `chmod +x "Link's Awakening DX HD"`
3. Ensure you have the required assets (Content and Data folders) from v1.0.0 as described in "Updating Source Code Assets"
4. Run the game: `./Link\'s\ Awakening\ DX\ HD`

## Important Notes

### Platform-Specific Limitations

**GBS Player Audio**: The original Windows version used DirectX-based audio (SharpDX.XAudio2) for the GBS music player. This functionality is currently disabled in the Linux/OpenGL build as it's not cross-platform compatible. The game will run without GBS music playback. Future updates may implement cross-platform audio using OpenAL or other alternatives.

**Editor Mode**: Some editor features that relied on Windows Forms file dialogs may not function in this Linux build. The main game functionality is fully preserved.

**Window Icon**: The application icon is not set in the Linux build (Windows-specific functionality).

### Platform Changes

This build targets:
- **Platform**: Linux ARM64
- **Graphics Backend**: OpenGL (via MonoGame.Framework.DesktopGL)
- **Runtime**: .NET 6.0
- **Build Method**: Cross-compilation from Windows or native compilation on Linux
- **Content Pipeline**: Always uses DesktopGL pipeline to compile shaders for OpenGL

**Important Note on Content Pipeline:**
The MonoGame Content Pipeline compiles shaders to the appropriate format for the target platform. With `MonoGamePlatform=DesktopGL`, shaders are compiled to GLSL (OpenGL Shading Language), not DirectX bytecode. The content pipeline runs on the build machine and requires native libraries (`libmojoshader_64.dll` on Windows) to perform shader translation. The project automatically copies these libraries from the NuGet packages during build to ensure shader compilation works correctly on Windows.

Previous Windows/DirectX-specific features have been replaced with cross-platform alternatives:
- DirectX → OpenGL
- Windows Forms MessageBox → Console error logging
- Windows Forms Application.Exit → MonoGame Game.Exit()
- Borderless fullscreen → MonoGame native fullscreen support

## Build Troubleshooting

If you experience the error **The command “dotnet tool restore” exited with code 1** then make sure the file **.config\dotnet-tools.json** isn't blocked. 

- To unblock all files in one go, run the included PowerShell script **"Unblock-All-Files.ps1"**.
- To unblock a single file: Right click, go to Properties, check Unblock, and click OK.


### Additional Troubleshooting for Linux ARM64

**Error: "Unable to load DLL 'libmojoshader_64.dll'" when compiling shaders**
- This occurs when the MonoGame Content Pipeline cannot find required native libraries for shader compilation
- The project automatically copies these native libraries from NuGet packages during build on Windows
- These libraries are needed to compile shaders to GLSL (OpenGL format), not DirectX
- Ensure you have the Content folder with shader files properly set up from v1.0.0 assets
- If the error persists:
  1. Try running `dotnet restore` to ensure all packages are downloaded
  2. Check that the MonoGame.Framework.Content.Pipeline package is correctly installed
  3. Verify you're using .NET 6.0 SDK or higher
- The shader compilation will produce OpenGL-compatible shaders regardless of the build platform

**Error: "The target framework 'net6.0' is out of support"**
- This is a warning, not an error. The build will still work
- Consider upgrading to .NET 8.0 LTS or later for long-term support
- The warning can be ignored for now as .NET 6.0 is still functional

**Build errors about missing Content or Data folders**
- Follow the steps in "Updating Source Code Assets" to properly migrate the asset files
- The Data folder must exist (can be empty) for the build to succeed

**Runtime error on Linux ARM64: "libgdiplus not found"**
- Install libgdiplus: `sudo apt-get install libgdiplus` (Debian/Ubuntu)
- Or equivalent package for your Linux distribution

**Runtime error: OpenGL/graphics issues**
- Ensure your system has OpenGL support
- Install Mesa drivers: `sudo apt-get install libgl1-mesa-glx libgl1-mesa-dri`
- Check OpenGL version: `glxinfo | grep "OpenGL version"`

**Permission denied when running the game**
- Make the executable runnable: `chmod +x "Link's Awakening DX HD"`
## About This Fork

I am a terrible programmer, but I have a love for this game. A ton of forks popped up, some with fixes, but nowhere were they all centralized. This fork attempted to find and implement all the various fixes and improvements spread across the other various forks. Once that was done, I started tackling the issues from the repository this was cloned from. And after that was done, I worked on anything else I could find that would make the game feel more like the original game.

Feel free to commit any potential fixes as a PR. There are no coding guidelines and any style is welcome as long as the code either fixes something broken or makes the game behave closer to the original. But do try to at least keep it neat.
