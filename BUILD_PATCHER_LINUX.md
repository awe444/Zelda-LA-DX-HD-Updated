# Building the LADXHD Patcher on Ubuntu 24.04 x64 (No Wine Required)

The patcher can be cross-compiled for Windows from Ubuntu 24.04 x64 using the .NET 8.0 SDK. The resulting executable is a standard Windows PE binary that you transfer to a Windows system to run.

## Prerequisites

Install the .NET 8.0 SDK (or later) on Ubuntu 24.04:

```bash
sudo apt update
sudo apt install -y dotnet-sdk-8.0
```

Verify the installation:

```bash
dotnet --version
```

## Building the Patcher

From the repository root, run:

```bash
cd ladxhd_patcher_source_code

dotnet publish -c Release -r win-x64 --self-contained true
```

The published output will be in:

```
ladxhd_patcher_source_code/bin/Release/net8.0-windows/win-x64/publish/
```

This folder contains `LADXHD_Patcher.exe` and all required runtime dependencies. Copy the **entire** `publish/` folder to your Windows system.

## Running on Windows

1. Copy the full `publish/` folder to the base folder of your v1.0.0 (or v1.1.4+) game installation on Windows.
2. Run `LADXHD_Patcher.exe` from that folder.
3. Select the desired **Platform** and **Target**, then press **Patch**.
4. When finished, the `publish/` folder can be deleted.

The `--self-contained true` flag bundles the .NET runtime into the output, so the target Windows machine does **not** need .NET installed separately.

## Build Options

| Option | Command | Notes |
|--------|---------|-------|
| Self-contained (recommended) | `dotnet publish -c Release -r win-x64 --self-contained true` | No .NET install needed on Windows |
| Framework-dependent | `dotnet publish -c Release -r win-x64 --self-contained false` | Requires [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) on Windows; smaller output |

## Notes

- **Wine is not required.** The .NET SDK handles cross-compilation natively via the `EnableWindowsTargeting` property in the project file.
- The patcher uses Windows Forms for its GUI and P/Invoke calls to Windows APIs. It will **only run on Windows**, but it can be **built** on any OS with the .NET SDK.
- Platform compatibility warnings (CA1416) during the build are expected and can be safely ignored — they simply note that WinForms APIs are Windows-only, which is the intended target.
