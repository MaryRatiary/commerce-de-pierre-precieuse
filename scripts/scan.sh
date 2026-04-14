#!/bin/bash

# Script de scan complet de sécurité
# Effectue : Gitleaks, SAST, Trivy, Dependency Check
# Utilisation : ./scripts/scan.sh [all|secrets|sast|container|deps]

set -e

SCAN_TYPE="${1:-all}"
REPORT_DIR="security-reports"
mkdir -p "$REPORT_DIR"

echo "🔐 ===== SECURITY SCAN REPORT ====="
echo "Date: $(date)"
echo "Type: $SCAN_TYPE"
echo "========================================"

# ============================================================================
# 1. GITLEAKS - Scan de secrets
# ============================================================================
scan_secrets() {
    echo ""
    echo "🔐 [1/5] Scanning for hardcoded secrets with Gitleaks..."
    
    if ! command -v gitleaks &> /dev/null; then
        echo "⚠️ Gitleaks not installed. Install with: brew install gitleaks"
        return 1
    fi
    
    gitleaks detect --source . --format json --report-path "$REPORT_DIR/gitleaks-report.json" || {
        SECRETS=$(jq '.Matches | length' "$REPORT_DIR/gitleaks-report.json" 2>/dev/null || echo "0")
        if [ "$SECRETS" -gt 0 ]; then
            echo "❌ CRITICAL: $SECRETS secrets detected!"
            jq '.Matches[] | "\(.Match) at \(.File):\(.LineNumber)"' "$REPORT_DIR/gitleaks-report.json"
            exit 1
        fi
    }
    echo "✅ No secrets detected"
}

# ============================================================================
# 2. SEMGREP - SAST Analysis
# ============================================================================
scan_sast() {
    echo ""
    echo "🛡️ [2/5] Running Semgrep SAST analysis..."
    
    if ! command -v semgrep &> /dev/null; then
        echo "⚠️ Semgrep not installed. Install with: brew install semgrep"
        return 1
    fi
    
    semgrep --config=p/security-audit \
            --config=p/owasp-top-ten \
            --json -o "$REPORT_DIR/semgrep-report.json" . || true
    
    ISSUES=$(jq '.results | length' "$REPORT_DIR/semgrep-report.json" 2>/dev/null || echo "0")
    echo "✅ Found $ISSUES potential security issues"
    
    if [ "$ISSUES" -gt 0 ]; then
        echo "⚠️ Review report: $REPORT_DIR/semgrep-report.json"
    fi
}

# ============================================================================
# 3. TRIVY - Container Image Scanning
# ============================================================================
scan_container() {
    echo ""
    echo "🐳 [3/5] Scanning container images with Trivy..."
    
    if ! command -v trivy &> /dev/null; then
        echo "⚠️ Trivy not installed. Install with: brew install aquasecurity/trivy/trivy"
        return 1
    fi
    
    # Scan Dockerfile backend
    echo "Scanning backend Dockerfile..."
    trivy config backend/Dockerfile \
        --severity HIGH,CRITICAL \
        --format json \
        --output "$REPORT_DIR/trivy-backend-config.json" || true
    
    # Scan Dockerfile frontend
    echo "Scanning frontend Dockerfile..."
    trivy config frontend/Dockerfile \
        --severity HIGH,CRITICAL \
        --format json \
        --output "$REPORT_DIR/trivy-frontend-config.json" || true
    
    echo "✅ Container config scanned"
}

# ============================================================================
# 4. DEPENDENCY CHECK - Dépendances vulnérables
# ============================================================================
scan_dependencies() {
    echo ""
    echo "📦 [4/5] Checking dependencies..."
    
    # Backend - OWASP Dependency Check
    if command -v dependency-check &> /dev/null; then
        echo "Scanning backend dependencies..."
        dependency-check --scan ./backend \
            --format JSON \
            --project "ecom-backend" \
            --out "$REPORT_DIR/dependency-check-backend.json" || true
    else
        echo "⚠️ OWASP Dependency-Check not installed"
    fi
    
    # Frontend - npm audit
    if [ -d "frontend" ] && [ -f "frontend/package.json" ]; then
        echo "Scanning frontend dependencies..."
        cd frontend
        npm audit --json > "../$REPORT_DIR/npm-audit-report.json" || true
        cd ..
        
        VULNS=$(jq '.metadata.vulnerabilities.total' "$REPORT_DIR/npm-audit-report.json" 2>/dev/null || echo "0")
        CRITICAL=$(jq '.metadata.vulnerabilities.critical' "$REPORT_DIR/npm-audit-report.json" 2>/dev/null || echo "0")
        echo "✅ Frontend: $VULNS vulnerabilities (Critical: $CRITICAL)"
    fi
}

# ============================================================================
# 5. BANDIT - Python security scanner
# ============================================================================
scan_bandit() {
    echo ""
    echo "🐍 [5/5] Running Bandit on Python files..."
    
    if ! command -v bandit &> /dev/null; then
        echo "⚠️ Bandit not installed. Install with: pip install bandit"
        return 1
    fi
    
    bandit -r . -f json -o "$REPORT_DIR/bandit-report.json" || true
    echo "✅ Python security scan completed"
}

# ============================================================================
# EXÉCUTION
# ============================================================================
case "$SCAN_TYPE" in
    all)
        scan_secrets
        scan_sast
        scan_container
        scan_dependencies
        scan_bandit
        ;;
    secrets)
        scan_secrets
        ;;
    sast)
        scan_sast
        ;;
    container)
        scan_container
        ;;
    deps)
        scan_dependencies
        ;;
    *)
        echo "❌ Unknown scan type: $SCAN_TYPE"
        echo "Valid options: all|secrets|sast|container|deps"
        exit 1
        ;;
esac

echo ""
echo "======================================"
echo "�� SCAN COMPLETE"
echo "📋 Reports saved to: $REPORT_DIR/"
echo "======================================"
