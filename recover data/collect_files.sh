#!/bin/bash

# Usage: ./collect_files.sh /path/to/main/dir "ext1 ext2 ext3"
# Example: ./collect_files.sh ./src "cs md"

MAIN_DIR="$1"
EXTENSIONS="$2"
OUTPUT_FILE="$(basename "$MAIN_DIR").temporal.txt"

# Remove old output if exists
> "$OUTPUT_FILE"

# Loop over extensions
for ext in $EXTENSIONS; do
  # Find files, sort them, and iterate
  find "$MAIN_DIR" -type f -name "*.$ext" | sort | while read -r file; do
    REL_PATH="${file#$MAIN_DIR/}"
    echo "===== $REL_PATH =====" >> "$OUTPUT_FILE"
    cat "$file" >> "$OUTPUT_FILE"
    echo -e "\n" >> "$OUTPUT_FILE"
  done
done

echo "Done! Collected files into $OUTPUT_FILE"