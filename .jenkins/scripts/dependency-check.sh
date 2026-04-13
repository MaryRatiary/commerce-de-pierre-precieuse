#!/bin/bash
set -e

echo "📦 Starting Dependency Vulnerability Scan..."

# Backend dependencies (.NET)
if [ -d "backend" ]; then
    echo "🔍 Checking .NET vulnerabilities..."
    cd backend/EcomApi
    dotnet list package --vulnerable || true
    cd ../..
fi

# Frontend dependencies (npm)
if [ -d "frontend" ]; then
    echo "🔍 Checking npm vulnerabilities..."
    cd frontend
    npm audit --audit-level=moderate --json > ../npm-audit-report.json || true
    cd ..
fi

# Generate SBOM with Syft
echo "📋 Generating Software Bill of Materials (SBOM)..."
if command -v syft &> /dev/null; then
    syft backend/EcomApi -o json > sbom-backend.json || true
    syft frontend -o json > sbom-frontend.json || true
else
    echo "⚠️  Syft not installed. Skipping SBOM generation."
fi

echo "✅ Dependency check completed"
