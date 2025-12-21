@echo off
REM Build only the game project (ProjectZ)
REM This is the most commonly needed build

echo ========================================
echo Building Zelda LADX HD - Game Only
echo ========================================
echo.

REM Check if we're in the right directory
if not exist "ladxhd_game_source_code" (
    echo ERROR: This script must be run from the repository root directory.
    echo Current directory: %CD%
    pause
    exit /b 1
)

REM Store the root directory
set ROOT_DIR=%CD%

cd "%ROOT_DIR%\ladxhd_game_source_code"

echo Checking for .NET 6.0 SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK not found. Please install .NET 6.0 SDK.
    echo See BUILD_WINDOWS.md for installation instructions.
    cd "%ROOT_DIR%"
    pause
    exit /b 1
)

echo Restoring dotnet tools...
dotnet tool restore
if errorlevel 1 (
    echo ERROR: Failed to restore dotnet tools.
    echo Try running: .\Unblock-All-Files.ps1
    cd "%ROOT_DIR%"
    pause
    exit /b 1
)

echo Restoring project dependencies...
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore project dependencies.
    cd "%ROOT_DIR%"
    pause
    exit /b 1
)

echo Building and publishing game...
dotnet publish -c Release -p:"PublishProfile=FolderProfile"
if errorlevel 1 (
    echo ERROR: Failed to build the game.
    echo Make sure game assets are set up correctly (see README.md).
    cd "%ROOT_DIR%"
    pause
    exit /b 1
)

echo.
echo ========================================
echo Game build completed successfully!
echo ========================================
echo.
echo Output location: %ROOT_DIR%\ladxhd_game_source_code\Publish\
echo Executable: %ROOT_DIR%\ladxhd_game_source_code\Publish\Link's Awakening DX HD.exe
echo.

cd "%ROOT_DIR%"
pause
