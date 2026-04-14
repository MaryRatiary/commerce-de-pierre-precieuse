#!/bin/bash
set -e

echo "🔐 Running Gitleaks - Secrets Detection"
echo "======================================="

# Install Gitleaks if not present
if ! command -v gitleaks &> /dev/null; then
    echo "Installing Gitleaks..."
    if [[ "$OSTYPE" == "darwin"* ]]; then
        brew install gitleaks
    else
        curl -L https://github.com/gitleaks/gitleaks/releases/download/v8.18.0/gitleaks-linux-x64 -o gitleaks
        chmod +x gitleaks
        sudo mv gitleaks /usr/local/bin/
    fi
fi

# Run Gitleaks
gitleaks detect \
    --source . \
    --verbose \
    --report-format json \
    --report-path gitleaks-report.json || EXIT_CODE=$?

# Parse results
if [ -f "gitleaks-report.json" ]; then
    SECRETS=$(jq '.Matches | length' gitleaks-report.json 2>/dev/null || echo "0")
    echo ""
    echo "📊 Results: Found ${SECRETS} potential secrets"
    
    if [ "$SECRETS" -gt 0 ]; then
        echo ""
        echo "⚠️  Detected secrets:"
        jq '.Matches[] | "\(.File): \(.Match)"' gitleaks-report.json
        exit 1
    else
        echo "✅ No secrets detected!"
    fi
else
    echo "✅ Scan completed successfully"
fi

exit ${EXIT_CODE:-0}
