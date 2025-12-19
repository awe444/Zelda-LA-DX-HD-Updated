#!/bin/bash
# Helper script to disable editor fonts for gameplay-only builds on Linux
# This comments out editor font references in Content.mgcb to avoid Windows font dependencies

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

# Comment out the editor font lines
sed -i 's|^\(#begin Content/Fonts/editor font.spritefont\)|#\1|' "$CONTENT_MGCB"
sed -i 's|^\(#begin Content/Fonts/editor mono font.spritefont\)|#\1|' "$CONTENT_MGCB"
sed -i 's|^\(#begin Content/Fonts/editor small mono font.spritefont\)|#\1|' "$CONTENT_MGCB"

# Also comment out the importer/processor lines that follow
sed -i '/^#begin Content\/Fonts\/editor.*font.spritefont$/,/^$/ {
    /^#begin/b
    /^$/b
    s/^/#/
}' "$CONTENT_MGCB"

echo "Editor fonts disabled. Backup saved to $CONTENT_MGCB.backup"
echo ""
echo "The build will now skip editor fonts and only support gameplay."
echo "To restore editor fonts, run: cp $CONTENT_MGCB.backup $CONTENT_MGCB"
