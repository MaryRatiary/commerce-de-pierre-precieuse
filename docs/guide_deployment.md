# Guide de Déploiement - CI/CD Sécurisé

## 🚀 Setup Initial

### 1. Configuration GitHub Repository

```bash
# Clone et navigation
cd /Users/RatiaryMario/e-com
git remote -v  # Vérifier remote

# Configurer les protections de branche
# Settings → Branches → Add rule
# Pattern: main
# ✅ Require pull request reviews before merging
# ✅ Require status checks to pass
# ✅ Require branches to be up to date
# ✅ Include administrators in restrictions
```

### 2. Configuration GitHub Secrets

```bash
# Via CLI GitHub (si installé)
gh secret set SONARQUBE_TOKEN -b "votre-token-sonarqube"
gh secret set GITHUB_PAGES_TOKEN -b "votre-pat-token"

# Ou via Web UI:
# Settings → Secrets and variables → Actions → New repository secret
```

### 3. Configuration Local Pre-commit Hooks

```bash
# Installation
pip install pre-commit
cd /Users/RatiaryMario/e-com

# Créer .pre-commit-config.yaml (voir section suivante)
# Installer les hooks
pre-commit install

# Test
pre-commit run --all-files
```

---

## 📝 Configuration Fichiers Requis

### .pre-commit-config.yaml

```yaml
repos:
  - repo: https://github.com/gitleaks/gitleaks
    rev: v8.18.0
    hooks:
      - id: gitleaks
        stages: [commit]
        args: ['--verbose', '--redact']

  - repo: https://github.com/returntocorp/semgrep
    rev: v1.45.0
    hooks:
      - id: semgrep
        language: python
        stages: [commit]
        args: ['--config=p/owasp-top-ten', '--json']

  - repo: https://github.com/pre-commit/pre-commit-hooks
    rev: v4.4.0
    hooks:
      - id: trailing-whitespace
      - id: end-of-file-fixer
      - id: check-yaml
      - id: check-added-large-files
        args: ['--maxkb=1000']
```

### .gitignore (Mises à jour)

```
# Secrets et credentials
*.key
*.pem
.env
.env.local
secrets.json

# Build artifacts
backend/bin/
backend/obj/
frontend/dist/
frontend/node_modules/

# Logs et rapports
*.log
reports/
coverage/
gitleaks-report.json
semgrep-*.json
npm-audit-report.json
trivy-results.sarif

# IDE
.vscode/
.idea/
*.swp
```

---

## 🔄 Workflows GitHub Actions

### Trigger Workflows

Les workflows se déclenchent automatiquement sur :
1. **Push** sur `main` ou `develop`
2. **Pull Request** vers `main` ou `develop`
3. Modifications dans les chemins respectifs

### Vérification Status

```bash
# Via GitHub CLI
gh workflow list
gh run list --workflow=ci-backend.yml
gh run view <run-id> --log

# Affichage status dans PR
# Badge dans README.md
```

---

## 📦 Artifacts et Reports

### Emplacements Artifacts

**GitHub Actions Tab:**
- Settings → Actions → General → Artifact retention days

**Archive Local:**
```bash
# Télécharger artifacts
gh run download <run-id> -D ./artifacts/

# Consulter les rapports
open artifacts/backend-build/
open artifacts/frontend-build/
```

### SBOM (Software Bill of Materials)

```bash
# Généré automatiquement pendant le pipeline

# Consulter
cat sbom-backend.json | jq '.components[]'
cat sbom-frontend.json | jq '.components[]'

# Upload vers repository
git add sbom-*.json
git commit -m "docs: update SBOM"
git push
```

---

## ✅ Validation Pipeline

### Test Local

```bash
# Backend
cd backend/EcomApi
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release

# Frontend
cd ../../frontend
npm install --frozen-lockfile
npm run lint
npm run build
npm test  # si configuré
```

### Vérification Scans

```bash
# Gitleaks
gitleaks detect --source . --verbose

# Semgrep
docker run --rm -v $(pwd):/src returntocorp/semgrep \
  semgrep --config=p/owasp-top-ten /src

# npm audit
cd frontend && npm audit --audit-level=moderate

# dotnet packages
cd ../backend/EcomApi && dotnet list package --vulnerable
```

---

## 🐳 Container Registry

### GHCR Push Manual

```bash
# Login
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Build
docker build -f backend/Dockerfile -t ghcr.io/YOUR-ORG/ecom-api:v1.0.0 backend/
docker build -f frontend/Dockerfile -t ghcr.io/YOUR-ORG/ecom-frontend:v1.0.0 frontend/

# Push
docker push ghcr.io/YOUR-ORG/ecom-api:v1.0.0
docker push ghcr.io/YOUR-ORG/ecom-frontend:v1.0.0

# Trivy scan
trivy image ghcr.io/YOUR-ORG/ecom-api:v1.0.0
```

---

## 📊 Monitoring et Rapports

### Dashboard GitHub

1. **Actions Tab**
   - Voir tous les workflows
   - Consulter logs détaillés
   - Re-run si nécessaire

2. **Security Tab**
   - Dependabot alerts
   - Code scanning results
   - Secret scanning alerts
   - Security advisories

### Rapports Périodiques

```bash
# Export rapports
gh run list --workflow=ci-backend.yml --json conclusion,status,createdAt

# Générer rapport manuel
cat > generate_report.sh << 'INNER_EOF'
#!/bin/bash
echo "=== Pipeline Status Report ==="
echo "Backend: $(gh run list --workflow=ci-backend.yml --limit=1 --json conclusion)"
echo "Frontend: $(gh run list --workflow=ci-frontend.yml --limit=1 --json conclusion)"
echo "=== Security Alerts ==="
gh api repos/{owner}/{repo}/security/dependabot/alerts
INNER_EOF

chmod +x generate_report.sh
./generate_report.sh
```

---

## 🔧 Jenkins Configuration (Optionnel)

### Setup Jenkins

```bash
# Si utilisant Jenkins localement (Docker)
docker run -d -p 8080:8080 \
  -e JENKINS_OPTS="--argumentsRealm.passwd.admin=admin" \
  jenkins/jenkins:latest

# Accès
# http://localhost:8080
# Admin / admin (changer le mot de passe)
```

### Configuration Jenkins Pipeline

1. **New Item** → Pipeline
2. **Pipeline** section:
   - Definition: Pipeline script from SCM
   - SCM: Git
   - Repository URL: `https://github.com/YOUR-ORG/e-com.git`
   - Script path: `.jenkins/Jenkinsfile`

3. **Build Triggers**
   - Poll SCM: `H/15 * * * *` (toutes les 15 min)
   - GitHub hook trigger

---

## 🚨 Troubleshooting

### Pipeline Fails

```bash
# 1. Vérifier logs
gh run view <run-id> --log | tail -50

# 2. Vérifier secrets
gh secret list

# 3. Vérifier branch protection
gh api repos/{owner}/{repo}/branches/main/protection

# 4. Vérifier status checks
gh api repos/{owner}/{repo}/commits/{ref}/check-runs
```

### Secret Leaked

```bash
# 1. Immédiat
git secret reveal 2>/dev/null || echo "Gitleaks: CHECK REPORT"

# 2. Rotate secret dans GitHub
# Settings → Secrets and variables → Delete → Create new

# 3. Force push (⚠️ Dangerous - Rewrite History)
git filter-branch --tree-filter 'rm -f sensitive-file' HEAD
git push origin --force-with-lease

# 4. Alternative: BFG Repo Cleaner
bfg --delete-files sensitive-file
```

### Performance Issues

```bash
# 1. Vérifier cache
# Actions → Settings → Actions → Cache management

# 2. Vérifier concurrency
# workflows: { group: '${{ github.workflow }}-${{ github.ref }}' }

# 3. Réduire artifacts
# retention-days: 5 (au lieu de 90)

# 4. Paralléliser jobs
# Stratégie matrix pour multi-versions
```

---

## 📋 Checklist Déploiement

- [ ] GitHub repository créé et configuré
- [ ] Branch protection rules activées
- [ ] GitHub Secrets configurés
- [ ] Workflows GitHub Actions copiés
- [ ] Fichiers de configuration pushés
- [ ] Pre-commit hooks installés localement
- [ ] Premier push fait → Workflows déclenchés
- [ ] Tous les checks passent
- [ ] Artifacts visibles dans GitHub
- [ ] Documentation mise à jour
- [ ] Équipe notifiée et formée

---

## 🎓 Formation Équipe

### Documentation à Partager
1. Guide sécurité (`docs/guide_securite.md`)
2. Analyse risques (`docs/analyse_risques_stride.md`)
3. Cahier charges (`docs/cahier_des_charges.md`)

### Commandes Essentielles

```bash
# Pour chaque développeur
echo "=== Setup Local Environment ==="
git clone https://github.com/YOUR-ORG/e-com.git
cd e-com

# Install pre-commit hooks
pip install pre-commit
pre-commit install

# Backend setup
cd backend/EcomApi
dotnet restore

# Frontend setup
cd ../../frontend
npm install --frozen-lockfile

echo "✅ Environment prêt!"
```

