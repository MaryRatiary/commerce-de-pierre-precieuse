#!/bin/bash
set -e

echo "🔐 Starting Secret Scanning with Gitleaks..."

GITLEAKS_VERSION="8.18.0"
GITLEAKS_BIN="gitleaks"

# Download gitleaks if not installed
if ! command -v $GITLEAKS_BIN &> /dev/null; then
    echo "📦 Installing Gitleaks $GITLEAKS_VERSION..."
    wget -q "https://github.com/gitleaks/gitleaks/releases/download/v${GITLEAKS_VERSION}/gitleaks-linux-x64" -O $GITLEAKS_BIN
    chmod +x $GITLEAKS_BIN
fi

# Run gitleaks
$GITLEAKS_BIN detect --source . \
    --report-path gitleaks-report.json \
    --report-format json \
    --exit-code 0 || true

# Parse results
if [ -f "gitleaks-report.json" ]; then
    MATCHES=$(jq '.Matches | length' gitleaks-report.json 2>/dev/null || echo "0")
    
    if [ "$MATCHES" -gt 0 ]; then
        echo "⚠️  WARNING: Found $MATCHES potential secrets!"
        jq '.Matches[] | {File: .File, Secret: .Secret, Match: .Match}' gitleaks-report.json
        exit 1
    else
        echo "✅ No secrets detected!"
        exit 0
    fi
fi
