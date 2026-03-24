# Android Build Pipeline

This document explains when and how the Android binary (APK) is compiled, and what happens when the patcher runs on Windows.

## Quick Answer

The Android game code is **compiled once during the release/publishing process** — NOT when the patcher runs on Windows. The patcher only patches game assets (Content/Data files) and repackages them into a pre-compiled APK.

## How It Works

### Phase 1: Android APK Compilation (Developer/Publisher Side)

When the developer publishes a new release, the Android APK is compiled from source:

```
ladxhd_game_source_code\publish.bat
```

This script runs (among other targets):

```batch
dotnet publish ProjectZ.Android\ProjectZ.Android.csproj -c Release -f net8.0-android
```

This compiles all the C# game code (`ProjectZ.Core` + `ProjectZ.Android`) into a .NET Android APK containing:
- Compiled .NET assemblies (the game logic)
- MonoGame framework libraries
- Android manifest, icons, splash screens

The compiled APK is then **stripped of game assets** (Content/Data files) and saved as `android_base.apk` (~69 MB). This base APK is embedded into the patcher as a resource.

### Phase 2: Patcher Build (Developer/Publisher Side)

The patcher itself is compiled with the pre-built `android_base.apk` embedded as a resource:

```
ladxhd_patcher_source_code/Resources/android_base.apk    — Pre-compiled APK (no assets)
ladxhd_patcher_source_code/Resources/patches_android.zip  — xdelta3 patches for assets
ladxhd_patcher_source_code/Resources/android_tools.zip    — zipalign, apksigner, Java, keystore
```

These resources are baked into the patcher executable at build time.

### Phase 3: Patcher Execution (End User on Windows)

When a user runs the patcher on Windows and selects "Android":

1. **Extract** `android_base.apk` from embedded resources (pre-compiled, contains game code but no assets)
2. **Patch assets** — apply xdelta3 binary patches to v1.0.0 Content/Data files to produce updated assets
3. **Inject assets** into the APK using 7-Zip
4. **ZipAlign** the APK (required by Android)
5. **Sign** the APK using an embedded keystore via `apksigner`
6. **Output** the final `zelda.ladxhd.apk`

**No compilation happens on the user's machine.** The patcher only handles asset patching and APK repackaging.

## When Do Source Code Changes Take Effect?

If you modify the C# source code (e.g., in `ProjectZ.Core` or `ProjectZ.Android`):

| Change Type | When It Takes Effect |
|-------------|---------------------|
| **Game logic** (C# code) | Must recompile the Android APK and rebuild the patcher with the new `android_base.apk` |
| **Game assets** (Content/Data files) | Must create new xdelta3 patches and rebuild the patcher with updated `patches_android.zip` |

### To apply source code changes to Android:

1. **Compile the Android APK** using `publish.bat` or manually:
   ```bash
   cd ladxhd_game_source_code
   dotnet publish ProjectZ.Android/ProjectZ.Android.csproj -c Release -f net8.0-android
   ```
   This requires the .NET 8.0 SDK with the `android` workload installed:
   ```bash
   dotnet workload install android
   ```

2. **Update `android_base.apk`** — replace the file in `ladxhd_patcher_source_code/Resources/` with the newly compiled APK (stripped of assets).

3. **Rebuild the patcher** — the new APK will be embedded as a resource:
   ```bash
   cd ladxhd_patcher_source_code
   dotnet publish -c Release -r win-x64 --self-contained true
   ```

4. **Distribute** the updated patcher to users.

## Summary Diagram

```
Source Code Changes
        │
        ▼
┌─────────────────────┐
│ dotnet publish       │  ◄── Developer compiles Android APK
│ ProjectZ.Android     │      (requires .NET 8.0 + android workload)
└────────┬────────────┘
         │
         ▼
   android_base.apk (pre-compiled, no assets)
         │
         ▼
┌─────────────────────┐
│ dotnet publish       │  ◄── Developer builds patcher
│ LADXHD_Patcher      │      (embeds android_base.apk as resource)
└────────┬────────────┘
         │
         ▼
   LADXHD_Patcher.exe (contains embedded APK + patches + tools)
         │
         ▼
┌─────────────────────┐
│ User runs patcher    │  ◄── End user on Windows
│ on Windows           │      (NO compilation — only patches assets
└────────┬────────────┘       and repackages the APK)
         │
         ▼
   zelda.ladxhd.apk (final APK with patched assets)
```

## Notes

- The patcher can be cross-compiled for Windows from Linux — see [BUILD_PATCHER_LINUX.md](BUILD_PATCHER_LINUX.md).
- Ubuntu's built-in `dotnet-sdk-8.0` apt package may lack the Android workload. Use [Microsoft's dotnet-install.sh](https://dotnet.microsoft.com/en-us/download/dotnet/scripts) script instead if `dotnet workload install android` fails.
- The `publish.bat` script in `ladxhd_game_source_code/` builds all platforms (Windows DX, Windows GL, Android, Linux x64, Linux Arm64).
