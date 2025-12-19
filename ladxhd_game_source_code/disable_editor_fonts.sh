#!/bin/bash
# Helper script to disable editor fonts for gameplay-only builds on Linux
# This removes editor font references from Content.mgcb to avoid Windows font dependencies

set -e

CONTENT_MGCB="Content/Content.mgcb"

if [ ! -f "$CONTENT_MGCB" ]; then
    echo "Error: $CONTENT_MGCB not found."
    echo "Make sure you have migrated the assets first (see README.md)."
    exit 1
fi

echo "Disabling editor fonts in $CONTENT_MGCB..."

# Create backup
cp "$CONTENT_MGCB" "$CONTENT_MGCB.backup"

# Remove editor font blocks entirely
# Each block starts with #begin and ends with a blank line
# We need to remove the entire block for each editor font

# Use awk to filter out the editor font blocks
awk '
BEGIN { skip = 0 }
/^#begin Content\/Fonts\/editor font\.spritefont/ { skip = 1; next }
/^#begin Content\/Fonts\/editor mono font\.spritefont/ { skip = 1; next }
/^#begin Content\/Fonts\/editor small mono font\.spritefont/ { skip = 1; next }
skip == 1 && /^$/ { skip = 0; next }
skip == 0 { print }
' "$CONTENT_MGCB.backup" > "$CONTENT_MGCB"

echo "Editor fonts disabled. Backup saved to $CONTENT_MGCB.backup"
echo ""
echo "The build will now skip editor fonts and only support gameplay."
echo "Editor font blocks have been removed from Content.mgcb"
echo "To restore editor fonts, run: cp $CONTENT_MGCB.backup $CONTENT_MGCB"
