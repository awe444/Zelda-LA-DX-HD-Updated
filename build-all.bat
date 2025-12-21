@echo off
REM Build all projects in the repository
REM This script builds the Game, Patcher, and Migrater projects
REM
REM This script builds the Release configuration.
REM For Debug builds or more options, see BUILD_WINDOWS.md

echo ========================================
echo Building Zelda LADX HD - All Projects
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

REM ========================================
echo [1/3] Building the Game (ProjectZ)...
echo ========================================
cd "%ROOT_DIR%\ladxhd_game_source_code"

echo Restoring dotnet tools...
dotnet tool restore
if errorlevel 1 (
    echo ERROR: Failed to restore dotnet tools. Check if .NET 6.0 SDK is installed.
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
    cd "%ROOT_DIR%"
    pause
    exit /b 1
)

echo Game build completed successfully!
echo Output: %ROOT_DIR%\ladxhd_game_source_code\Publish\
echo.

REM ========================================
echo [2/3] Building the Patcher...
echo ========================================
cd "%ROOT_DIR%\ladxhd_patcher_source_code"

REM Try to find MSBuild
set MSBUILD_PATH=

REM Check common MSBuild locations
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
) else (
    REM Try msbuild from PATH
    where msbuild >nul 2>&1
    if errorlevel 1 (
        echo ERROR: MSBuild not found. Please ensure Build Tools for Visual Studio 2022 is installed.
        echo See BUILD_WINDOWS.md for installation instructions.
        cd "%ROOT_DIR%"
        pause
        exit /b 1
    ) else (
        set "MSBUILD_PATH=msbuild"
    )
)

echo Using MSBuild: %MSBUILD_PATH%
"%MSBUILD_PATH%" LADXHD_Patcher.sln /p:Configuration=Release /v:minimal
if errorlevel 1 (
    echo ERROR: Failed to build the Patcher.
    cd "%ROOT_DIR%"
    pause
    exit /b 1
)

echo Patcher build completed successfully!
echo Output: %ROOT_DIR%\ladxhd_patcher_source_code\bin\Release\
echo.

REM ========================================
echo [3/3] Building the Migrater...
echo ========================================
cd "%ROOT_DIR%\ladxhd_migrate_source_code"

"%MSBUILD_PATH%" LADXHD_Migrater.sln /p:Configuration=Release /v:minimal
if errorlevel 1 (
    echo ERROR: Failed to build the Migrater.
    cd "%ROOT_DIR%"
    pause
    exit /b 1
)

echo Migrater build completed successfully!
echo Output: %ROOT_DIR%\ladxhd_migrate_source_code\bin\Release\
echo.

REM ========================================
echo ========================================
echo All builds completed successfully!
echo ========================================
echo.
echo Build outputs:
echo   Game:     %ROOT_DIR%\ladxhd_game_source_code\Publish\
echo   Patcher:  %ROOT_DIR%\ladxhd_patcher_source_code\bin\Release\
echo   Migrater: %ROOT_DIR%\ladxhd_migrate_source_code\bin\Release\
echo.

cd "%ROOT_DIR%"
pause
