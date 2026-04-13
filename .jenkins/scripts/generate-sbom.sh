#!/bin/bash
set -e

echo "📋 Generating Software Bill of Materials (SBOM)..."

SOURCE_PATH=${1:-.}
OUTPUT_FORMAT=${2:-json}

# Check if syft is installed
if ! command -v syft &> /dev/null; then
    echo "📦 Installing Syft..."
    curl -sSL https://raw.githubusercontent.com/anchore/syft/main/install.sh | sh -s -- -b /usr/local/bin
fi

# Generate SBOM
echo "🔍 Scanning: $SOURCE_PATH"
syft "$SOURCE_PATH" -o "$OUTPUT_FORMAT" > "sbom-$(basename $SOURCE_PATH).${OUTPUT_FORMAT}"

# Also generate CycloneDX format
syft "$SOURCE_PATH" -o cyclonedx > "sbom-$(basename $SOURCE_PATH).xml"

echo "✅ SBOM generation completed!"
ls -la sbom-*
