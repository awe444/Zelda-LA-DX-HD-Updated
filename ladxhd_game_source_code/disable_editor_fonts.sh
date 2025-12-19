#!/bin/bash
# Helper script to disable editor fonts for gameplay-only builds on Linux
# This removes editor font references from Content.mgcb to avoid Windows font dependencies

set -e

CONTENT_MGCB="Content/Content.mgcb"

echo "========================================="
echo "Editor Font Disabler Script"
echo "========================================="
echo ""

if [ ! -f "$CONTENT_MGCB" ]; then
    echo "❌ ERROR: $CONTENT_MGCB not found."
    echo "Make sure you have migrated the assets first (see README.md)."
    echo "Current directory: $(pwd)"
    echo "Looking for: $(readlink -f "$CONTENT_MGCB" 2>/dev/null || echo "$CONTENT_MGCB")"
    exit 1
fi

echo "✓ Found Content.mgcb at: $CONTENT_MGCB"
echo ""

# Show what we're looking for
echo "Searching for editor font blocks to remove:"
echo "  1. (Content/)?Fonts/editor font.spritefont"
echo "  2. (Content/)?Fonts/editor mono font.spritefont"
echo "  3. (Content/)?Fonts/editor small mono font.spritefont"
echo ""

# Check what's in the file before modification
# Note: paths may be either "Fonts/..." or "Content/Fonts/..."
FONT1_COUNT=$(grep -cE "^#begin (Content/)?Fonts/editor font\.spritefont" "$CONTENT_MGCB" || true)
FONT2_COUNT=$(grep -cE "^#begin (Content/)?Fonts/editor mono font\.spritefont" "$CONTENT_MGCB" || true)
FONT3_COUNT=$(grep -cE "^#begin (Content/)?Fonts/editor small mono font\.spritefont" "$CONTENT_MGCB" || true)

echo "Before modification:"
echo "  - 'editor font.spritefont' blocks found: $FONT1_COUNT"
echo "  - 'editor mono font.spritefont' blocks found: $FONT2_COUNT"
echo "  - 'editor small mono font.spritefont' blocks found: $FONT3_COUNT"
echo ""

if [ "$FONT1_COUNT" -eq 0 ] && [ "$FONT2_COUNT" -eq 0 ] && [ "$FONT3_COUNT" -eq 0 ]; then
    echo "✓ No editor fonts found - already disabled or not present."
    echo "Nothing to do!"
    exit 0
fi

# Create backup
echo "Creating backup: $CONTENT_MGCB.backup"
cp "$CONTENT_MGCB" "$CONTENT_MGCB.backup"
echo "✓ Backup created"
echo ""

# Remove editor font blocks entirely
# Each block starts with #begin and ends with a blank line
echo "Removing editor font blocks..."

# Use awk to filter out the editor font blocks with verbose output
# Each block structure is:  #begin → properties → /build: → blank line → next #begin
# Note: File may have Windows (CRLF) or Unix (LF) line endings
awk '
BEGIN { 
    skip = 0
    removed_blocks = 0
    current_block = ""
    prev_was_build = 0
}
# Check if this is an editor font block we want to remove
# Match with optional whitespace/CR at end
/^#begin (Content\/)?Fonts\/editor font\.spritefont[[:space:]]*$/ { 
    skip = 1
    removed_blocks++
    current_block = "editor font.spritefont"
    print "  → Removing block: " current_block > "/dev/stderr"
    prev_was_build = 0
    next 
}
/^#begin (Content\/)?Fonts\/editor mono font\.spritefont[[:space:]]*$/ { 
    skip = 1
    removed_blocks++
    current_block = "editor mono font.spritefont"
    print "  → Removing block: " current_block > "/dev/stderr"
    prev_was_build = 0
    next 
}
/^#begin (Content\/)?Fonts\/editor small mono font\.spritefont[[:space:]]*$/ { 
    skip = 1
    removed_blocks++
    current_block = "editor small mono font.spritefont"
    print "  → Removing block: " current_block > "/dev/stderr"
    prev_was_build = 0
    next 
}
# While skipping, track if we see the /build: line
skip == 1 && /^\/build:/ {
    prev_was_build = 1
    next
}
# While skipping, if we see a blank line after /build:, the block is complete
skip == 1 && /^[[:space:]]*$/ && prev_was_build == 1 { 
    skip = 0
    prev_was_build = 0
    print "    ✓ Block removed: " current_block > "/dev/stderr"
    current_block = ""
    next 
}
# Any other line while skipping - just skip it
skip == 1 { 
    prev_was_build = 0
    next 
}
# Not skipping - print the line and reset flag
skip == 0 { 
    prev_was_build = 0
    print 
}
END {
    print "" > "/dev/stderr"
    print "Total blocks removed: " removed_blocks > "/dev/stderr"
}
' "$CONTENT_MGCB.backup" > "$CONTENT_MGCB"

echo ""

# Verify the changes
FONT1_AFTER=$(grep -cE "^#begin (Content/)?Fonts/editor font\.spritefont" "$CONTENT_MGCB" || true)
FONT2_AFTER=$(grep -cE "^#begin (Content/)?Fonts/editor mono font\.spritefont" "$CONTENT_MGCB" || true)
FONT3_AFTER=$(grep -cE "^#begin (Content/)?Fonts/editor small mono font\.spritefont" "$CONTENT_MGCB" || true)

echo "After modification:"
echo "  - 'editor font.spritefont' blocks remaining: $FONT1_AFTER"
echo "  - 'editor mono font.spritefont' blocks remaining: $FONT2_AFTER"
echo "  - 'editor small mono font.spritefont' blocks remaining: $FONT3_AFTER"
echo ""

if [ "$FONT1_AFTER" -eq 0 ] && [ "$FONT2_AFTER" -eq 0 ] && [ "$FONT3_AFTER" -eq 0 ]; then
    echo "✅ SUCCESS: All editor font blocks removed!"
else
    echo "⚠️  WARNING: Some editor font blocks may still remain!"
    echo "This could indicate the .mgcb file format is different than expected."
    echo ""
    echo "Remaining references:"
    grep "editor.*font\.spritefont" "$CONTENT_MGCB" || echo "  (none found with grep)"
fi

echo ""
echo "========================================="
echo "Summary:"
echo "  - Backup saved: $CONTENT_MGCB.backup"
echo "  - Modified: $CONTENT_MGCB"
echo "  - To restore: cp $CONTENT_MGCB.backup $CONTENT_MGCB"
echo "========================================="
echo ""
echo "You can now run the build script: ./publish_linux.sh"
