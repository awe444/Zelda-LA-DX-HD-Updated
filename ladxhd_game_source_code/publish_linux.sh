#!/bin/bash
set -e  # Exit on any command failure

# Build script for Linux ARM64 (Ubuntu 25.04 aarch64)

echo "Building Zelda LA DX HD for Linux ARM64..."

# Restore .NET tools
dotnet tool restore

# Build for Linux ARM64
echo "Building for Linux ARM64..."
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -o "./Publish/linux-arm64"

echo "Build complete! Binary is in ./Publish/linux-arm64/"
