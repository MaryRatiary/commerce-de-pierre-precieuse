# 🏪 E-Commerce Platform - CI/CD Sécurisé

[![Backend CI/CD](https://github.com/YOUR-ORG/e-com/actions/workflows/ci-backend.yml/badge.svg)](https://github.com/YOUR-ORG/e-com/actions/workflows/ci-backend.yml)
[![Frontend CI/CD](https://github.com/YOUR-ORG/e-com/actions/workflows/ci-frontend.yml/badge.svg)](https://github.com/YOUR-ORG/e-com/actions/workflows/ci-frontend.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Plateforme e-commerce complète avec **pipeline CI/CD sécurisé** utilisant GitHub Actions et Jenkins, incluant scans de sécurité automatisés, gestion des vulnérabilités et SBOM.

## 📋 Table des Matières

- [Architecture](#-architecture)
- [Features](#-features)
- [Technologies](#-technologies)
- [Quick Start](#-quick-start)
- [Pipeline CI/CD](#-pipeline-cicd)
- [Sécurité](#-sécurité)
- [Documentation](#-documentation)
- [Contribution](#-contribution)

---

## 🏗️ Architecture

### Global Architecture
```
Developer Push → GitHub Repository
    ↓
GitHub Actions CI/CD Pipeline
    ├─→ [Security Scans] Gitleaks, Semgrep, npm audit
    ├─→ [Build] Backend (.NET), Frontend (React)
    ├─→ [Tests] Unit tests, Linting
    ├─→ [Container Scan] Trivy
    └─→ [Quality Gate] SonarQube, Code Coverage
         ↓
Artifacts → GitHub Container Registry (GHCR)
    ↓
Jenkins (Optionnel) → Manual Validation
    ↓
✅ Ready for Deployment
```

### Composants

| Composant | Technologie | Rôle |
|-----------|-------------|------|
| **Backend API** | .NET 8.0 + Entity Framework | REST API, Business Logic |
| **Frontend** | React 18 + Vite + TailwindCSS | User Interface |
| **Database** | SQL Server / PostgreSQL | Data Persistence |
| **CI/CD** | GitHub Actions + Jenkins | Automation |
| **Security** | Gitleaks, Semgrep, Trivy | Vulnerability Scanning |
| **Registry** | GitHub Container Registry | Image Storage |

---

## ✨ Features

### 🔐 Sécurité
- ✅ Secret scanning avec **Gitleaks** (détection API keys, tokens)
- ✅ SAST avec **Semgrep** (OWASP Top 10, CWE Top 25)
- ✅ Dependency scanning (**npm audit**, **dotnet list --vulnerable**)
- ✅ Container scanning avec **Trivy**
- ✅ SBOM generation avec **Syft**
- ✅ Branch protection et PR reviews
- ✅ Secrets management via GitHub Secrets

### 🚀 Automation
- ✅ Auto-build et tests sur chaque push
- ✅ Parallel jobs (backend/frontend)
- ✅ Automated quality gates
- ✅ Artifact archival et retention
- ✅ Slack/email notifications
- ✅ Jenkins pipeline alternatif

### 📊 Quality & Compliance
- ✅ Code coverage > 80%
- ✅ ESLint + dotnet format
- ✅ SonarQube integration (optionnel)
- ✅ OWASP & CWE compliance
- ✅ Audit logs complets

---

## 🛠️ Technologies

### Backend
- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core
- **Authentication**: JWT
- **API**: RESTful
- **Database**: SQL Server compatible

### Frontend
- **Framework**: React 18
- **Build Tool**: Vite
- **Styling**: TailwindCSS
- **Package Manager**: npm
- **Linting**: ESLint

### DevSecOps
- **CI/CD**: GitHub Actions + Jenkins
- **Container**: Docker
- **Registry**: GitHub Container Registry
- **Scanning**: Gitleaks, Semgrep, Trivy, OWASP Dependency Check
- **SBOM**: Syft
- **Monitoring**: GitHub Security Alerts

---

## �� Quick Start

### Prérequis
```bash
- Git
- GitHub account avec 2FA activé
- Docker (optionnel pour tests locaux)
- Node.js 18+ (frontend)
- .NET 8.0 SDK (backend)
- pip (pour pre-commit hooks)
```

### 1️⃣ Installation Locale

```bash
# Clone repository
git clone https://github.com/YOUR-ORG/e-com.git
cd e-com

# Setup pre-commit hooks (IMPORTANT pour sécurité!)
pip install pre-commit
pre-commit install

# Backend setup
cd backend/EcomApi
dotnet restore
dotnet build --configuration Release

# Frontend setup
cd ../../frontend
npm install --frozen-lockfile

echo "✅ Local environment ready!"
```

### 2️⃣ Configuration GitHub

```bash
# Setup branch protection
# Go to Settings → Branches → Add rule
# Pattern: main
# ✅ Require pull request reviews (2)
# ✅ Require status checks
# ✅ Require branches to be up to date

# Configure secrets
# Settings → Secrets and variables → Actions
gh secret set SONARQUBE_TOKEN -b "your-token"
gh secret set GITHUB_PAGES_TOKEN -b "your-pat"
```

### 3️⃣ Premier Push

```bash
# Test local
cd backend/EcomApi && dotnet test
cd ../../frontend && npm run lint && npm run build

# Push
git add .
git commit -m "chore: initial commit with CI/CD"
git push origin main

# Watch workflows
gh run list --workflow=ci-backend.yml
```

---

## 🔄 Pipeline CI/CD

### 📊 Backend Workflow (`ci-backend.yml`)

| Stage | Tool | Action | Timeout |
|-------|------|--------|---------|
| 🔐 Secrets | Gitleaks | Détecte secrets | 5 min |
| 🛡️ SAST | Semgrep | Analyse statique | 10 min |
| 📦 Dependencies | OWASP DC | Scan vulnérabilités | 10 min |
| 🔨 Build | dotnet | Compilation | 10 min |
| 🧪 Tests | dotnet test | Unit tests | 10 min |
| 🐳 Container | Trivy | Image scan | 10 min |
| ✅ Quality | SonarQube | Gate validation | 5 min |

**Total**: ~60 minutes

### 📊 Frontend Workflow (`ci-frontend.yml`)

| Stage | Tool | Action | Timeout |
|-------|------|--------|---------|
| 🔐 Secrets | Gitleaks | Détecte secrets | 5 min |
| 🛡️ SAST | Semgrep | JS/TS analysis | 10 min |
| 📦 Audit | npm audit | Dependencies | 5 min |
| 📋 SBOM | Syft | Bill of Materials | 5 min |
| 🔨 Build | npm run build | Vite build | 5 min |
| 🐳 Container | Trivy | Image scan | 10 min |
| ✅ Quality | ESLint | Code quality | 5 min |

**Total**: ~45 minutes

### Status Checks

```bash
# Voir les status checks requis
gh api repos/{owner}/{repo}/branches/main/protection/required_status_checks

# Re-run failed checks
gh run rerun <run-id>

# Cancel running workflow
gh run cancel <run-id>
```

---

## 🔒 Sécurité

### 🔐 Secrets Management

**GitHub Secrets:**
```bash
# Manage via CLI
gh secret list
gh secret set MY_SECRET -b "value"
gh secret delete MY_SECRET
```

**Best Practices:**
- ✅ Jamais commit de secrets
- ✅ Rotation tous les 90 jours
- ✅ Pre-commit hooks pour Gitleaks
- ✅ Use GitHub Secrets for CI/CD
- ✅ Audit logs réguliers

### 🛡️ Compliance

**OWASP Top 10 2021:**
- A01 → Branch protection, access control
- A02 → Secret scanning, encryption
- A05 → SAST scanning, injection prevention
- A06 → Dependency scanning, vulnerable components
- A10 → SBOM, supply chain security

**CWE Top 25:**
- CWE-79 (XSS) → Semgrep rules
- CWE-89 (SQL Injection) → Semgrep rules
- CWE-798 (Hardcoded Credentials) → Gitleaks

### 📋 Vulnerability Response

**Critical Vulnerability:**
1. ⚠️ GitHub alert (Dependabot, Code Scanning)
2. 🔔 Slack notification
3. 📝 Create issue (P1)
4. 🔧 Fix + test locally
5. ✅ Push → Pipeline validation
6. 🎯 Merge + deploy

---

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| [cahier_des_charges.md](docs/cahier_des_charges.md) | Spécifications du projet |
| [analyse_risques_stride.md](docs/analyse_risques_stride.md) | Threat modeling STRIDE |
| [guide_securite.md](docs/guide_securite.md) | Policies & best practices |
| [guide_deployment.md](docs/guide_deployment.md) | Setup & deployment |

---

## 🔧 Configuration

### `.github/workflows/`
- `ci-backend.yml` - Backend pipeline
- `ci-frontend.yml` - Frontend pipeline

### `.jenkins/`
- `Jenkinsfile` - Main pipeline
- `scripts/` - Reusable scripts
  - `scan-secrets.sh` - Gitleaks
  - `sast-analysis.sh` - Semgrep
  - `dependency-check.sh` - Vulnerabilities

### `security/`
- `policies.json` - Security policies
- `sonarqube-config.xml` - Quality configuration

### `.pre-commit-config.yaml`
- Gitleaks hook
- Semgrep hook
- Standard hooks

---

## 📊 Reports & Artifacts

### GitHub Actions

```bash
# List workflows
gh workflow list

# View runs
gh run list --workflow=ci-backend.yml

# Download artifacts
gh run download <run-id> -D ./artifacts/

# View logs
gh run view <run-id> --log
```

### SBOM

```bash
# Backend SBOM
cat sbom-backend.json | jq '.components[]'

# Frontend SBOM
cat sbom-frontend.json | jq '.components[]'

# Upload to repository
git add sbom-*.json
git commit -m "docs: update SBOM"
git push
```

### Security Reports

```bash
# Dependabot alerts
gh api repos/{owner}/{repo}/security/dependabot/alerts

# Code scanning
gh api repos/{owner}/{repo}/code-scanning/alerts

# Secret scanning
gh api repos/{owner}/{repo}/secret-scanning/alerts
```

---

## 🚨 Troubleshooting

### Workflow Failed

```bash
# View logs
gh run view <run-id> --log | tail -100

# Check secrets
gh secret list

# Validate branch protection
gh api repos/{owner}/{repo}/branches/main/protection
```

### Secret Exposed

```bash
# 1. Immediate rotation
gh secret delete MY_SECRET
gh secret set MY_SECRET -b "new-value"

# 2. Audit history
git log --all --pretty=format:"%H %s" | grep -i secret

# 3. Rewrite history (if necessary)
git filter-branch --tree-filter 'rm -f sensitive-file' HEAD
git push origin --force-with-lease
```

### Performance Issues

```bash
# Check cache usage
# Actions → Settings → Actions → Cache management

# View artifact storage
gh api repos/{owner}/{repo}/actions/runs?per_page=10 | jq '.run_artifacts'

# Optimize cache
# Increase cache-dependency-path specificity
```

---

## 🤝 Contribution

### Workflow

1. **Create branch**
   ```bash
   git checkout -b feature/your-feature
   ```

2. **Commit changes** (pre-commit hooks trigger)
   ```bash
   git add .
   git commit -m "feat: description"
   ```

3. **Push & create PR**
   ```bash
   git push origin feature/your-feature
   # Open PR on GitHub
   ```

4. **Checks & reviews**
   - ✅ All CI checks must pass
   - ✅ 2 approvals required
   - ✅ No conflicts with main

5. **Merge**
   ```bash
   # After approval
   # GitHub UI or CLI: gh pr merge <number>
   ```

### Code Standards

- **Backend**: .NET code style (dotnet format)
- **Frontend**: ESLint config
- **Tests**: > 80% coverage
- **Commits**: Conventional commits (feat, fix, docs, etc.)

---

## 📈 Monitoring

### GitHub Security Dashboard

```
Settings → Security → Code scanning
Settings → Security → Dependabot alerts
Settings → Security → Secret scanning
```

### Metrics

```bash
# Build success rate
gh run list --limit=100 | grep -c "completed"

# Average build time
gh run list --limit=20 | jq '.[] | .duration_ms' | awk '{sum+=$1} END {print sum/NR/1000 " seconds"}'
```

---

## 📝 License

MIT License - See [LICENSE](LICENSE) file

---

## 👥 Team

- **DevSecOps Lead**: [Your Name]
- **Backend Lead**: [Team Member]
- **Frontend Lead**: [Team Member]

---

## 📞 Support

- 📧 Email: team@company.com
- 💬 Slack: #devops-support
- 🐛 Issues: GitHub Issues
- 📖 Wiki: GitHub Wiki

---

## 🔗 Related Links

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Jenkins Documentation](https://www.jenkins.io/doc/)
- [OWASP Top 10](https://owasp.org/Top10/)
- [CWE Top 25](https://cwe.mitre.org/top25/)

---

**Last Updated**: April 13, 2026  
**Status**: ✅ Production Ready

# devops
