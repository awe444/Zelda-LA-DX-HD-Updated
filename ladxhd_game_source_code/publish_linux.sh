#!/bin/bash
# Build script for Linux (Ubuntu)

echo "Building Zelda LA DX HD for Linux..."

# Restore .NET tools
dotnet tool restore

# Build for Linux ARM64 (native)
echo "Building for Linux ARM64..."
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -o "./Publish/linux-arm64"

# Optionally build for Linux x64 as well
echo "Building for Linux x64..."
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o "./Publish/linux-x64"

echo "Build complete! Binaries are in ./Publish/"
