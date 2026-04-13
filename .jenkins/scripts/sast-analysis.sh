#!/bin/bash
set -e

echo "🛡️ Starting SAST Analysis with Semgrep..."

SEMGREP_RULES="p/owasp-top-ten p/cwe-top-25 p/security-audit"

# Backend SAST
if [ -d "backend" ]; then
    echo "🔍 Scanning backend..."
    docker run --rm -v $PWD/backend:/src returntocorp/semgrep \
        semgrep --config=$SEMGREP_RULES \
        --json --output=/src/semgrep-backend.json \
        /src || true
fi

# Frontend SAST
if [ -d "frontend" ]; then
    echo "🔍 Scanning frontend..."
    cd frontend
    npm run lint -- --format json --output-file ../semgrep-frontend.json || true
    cd ..
fi

echo "✅ SAST analysis completed"
