# Guide de Sécurité - CI/CD Pipeline

## 🔐 Principes de Sécurité

### 1. Zero Trust Architecture
- Vérifier chaque changement de code
- Authentifier toutes les actions
- Auditer toutes les opérations

### 2. Least Privilege
- Permissions minimales sur les secrets
- Accès restreint aux artifacts
- Roles-based access control (RBAC)

### 3. Defense in Depth
- Plusieurs couches de sécurité
- Scans à plusieurs étapes
- Validations redondantes

---

## 🔑 Gestion des Secrets

### Configuration GitHub Secrets

```bash
# Secrets requis dans Settings → Secrets and variables → Actions

1. GITHUB_PAGES_TOKEN
   - Personal Access Token (PAT)
   - Scope: repo, workflow
   - Rotation: Tous les 90 jours

2. SONARQUBE_TOKEN
   - Token SonarQube
   - Scope: Analysis

3. REGISTRY_USERNAME
   - GHCR credentials
   - Optionnel pour repo public

4. REGISTRY_PASSWORD
   - GHCR token
   - Optionnel pour repo public
```

### Best Practices Secrets
- ✅ Utiliser les secrets GitHub, jamais en dur
- ✅ Rotation régulière (90j max)
- ✅ Logs masqués automatiquement
- ✅ Audit des accès secrets
- ✅ Pre-commit hooks pour Gitleaks

### Commande Pre-commit Local

```bash
# Installation pre-commit framework
pip install pre-commit

# Configuration dans .pre-commit-config.yaml
repos:
  - repo: https://github.com/gitleaks/gitleaks
    rev: v8.18.0
    hooks:
      - id: gitleaks
        stages: [commit]
        args: ['--verbose', '--redact']
```

---

## 🛡️ Scans de Sécurité

### SAST (Static Application Security Testing)

**Semgrep Configuration:**
```yaml
# .semgrep.yml
rules:
  - id: owasp-top-ten
  - id: cwe-top-25
  - id: security-audit
  - id: javascript  # Frontend
  - id: python      # Backend si applicable
```

**Règles appliquées:**
- Injection SQL
- XSS (Cross-Site Scripting)
- CSRF (Cross-Site Request Forgery)
- Hardcoded credentials
- Insecure dependencies

### Secret Scanning

**Gitleaks detect:**
```bash
gitleaks detect --source . --verbose \
  --config ~/.gitleaks/config.toml \
  --exit-code 0
```

**Patterns détectés:**
- API Keys (AWS, Azure, GCP)
- Database credentials
- Private keys (RSA, PGP)
- OAuth tokens
- Slack/Discord webhooks

### Dependency Scanning

**Backend (.NET):**
```bash
dotnet list package --vulnerable --verbosity normal
# Affiche vulnérabilités packages NuGet
```

**Frontend (npm):**
```bash
npm audit --audit-level=moderate
# Recommandé: moderate, strict pour prod
```

**SBOM Generation (Software Bill of Materials):**
```bash
# Avec Syft
syft backend/EcomApi -o json > sbom-backend.json
syft frontend -o cyclonedx > sbom-frontend.xml
```

### Container Scanning

**Trivy scan:**
```bash
# Image scan
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
  aquasec/trivy image \
  --severity HIGH,CRITICAL \
  --format json \
  ghcr.io/your-org/ecom-api:latest

# Filesystem scan
trivy fs ./backend/EcomApi
```

---

## ✅ Quality Gates

### Critères de passage

| Critère | Backend | Frontend |
|---------|---------|----------|
| Gitleaks | 0 secrets | 0 secrets |
| SAST | 0 HIGH+ | 0 CRITICAL |
| Coverage | > 80% | > 70% |
| Dependencies | 0 CRITICAL CVE | 0 CRITICAL CVE |
| Container | 0 CRITICAL | 0 CRITICAL |
| Build time | < 30 min | < 20 min |

### Exemples d'échecs

❌ **Secrets detected** → Reject + Notification  
❌ **Critical vulnerability** → Reject + Alert  
❌ **Build failure** → Reject + Logs  
❌ **Test coverage < 80%** → Warning + Artifact  

---

## 🚀 Déploiement Sécurisé

### Image Registry

**GitHub Container Registry (GHCR):**
```bash
# Authentication
echo ${{ secrets.GITHUB_TOKEN }} | \
  docker login ghcr.io -u ${{ github.actor }} --password-stdin

# Push image
docker tag ecom-api:latest ghcr.io/org/ecom-api:latest
docker push ghcr.io/org/ecom-api:latest

# Pull image
docker pull ghcr.io/org/ecom-api:latest
```

### Image Signing (Cosign)

```bash
# Generate signing key
cosign generate-key-pair

# Sign image after push
cosign sign --key cosign.key ghcr.io/org/ecom-api:latest

# Verify signature
cosign verify --key cosign.pub ghcr.io/org/ecom-api:latest
```

---

## 📋 Checklist Sécurité

### Avant Push
- [ ] Aucun secret en dur dans le code
- [ ] Tests locaux passent
- [ ] Linting passé
- [ ] Branch à jour avec main

### Pull Request
- [ ] 2 reviews minimums
- [ ] All checks passed
- [ ] Pas de dépendances dangereuses
- [ ] SAST scan clean

### Post-Merge
- [ ] CI/CD pipeline complet réussi
- [ ] Artifacts archivés
- [ ] Rapports de sécurité consultés
- [ ] Notifications envoyées

---

## 🔔 Alertes et Monitoring

### GitHub Security Alerts

Activé automatiquement pour:
- Dependabot alerts
- Code scanning alerts
- Secret scanning alerts
- Security advisories

### Notification Configuration

```yaml
# Slack webhook (optionnel)
- name: Notify Slack on failure
  if: failure()
  uses: slackapi/slack-github-action@v1
  with:
    payload: |
      {
        "text": "❌ Pipeline Failed: ${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}"
      }
  env:
    SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK }}
```

---

## 🐛 Incident Response

### Si Secret Compromis

1. ✅ Rotate le secret immédiatement
2. ✅ Audit usage historique
3. ✅ Notifier l'équipe
4. ✅ Vérifier logs d'accès
5. ✅ Documenter l'incident

### Si Vulnérabilité Détectée

1. ✅ Isoler le commit
2. ✅ Créer issue de correction
3. ✅ Assign priorité (P1-P4)
4. ✅ Fix + test + merge
5. ✅ Documenter correction

---

## 📚 Références

- [OWASP Top 10 2021](https://owasp.org/Top10/)
- [CWE Top 25](https://cwe.mitre.org/top25/)
- [GitHub Security Best Practices](https://docs.github.com/en/code-security)
- [NIST Secure Software Development Framework](https://csrc.nist.gov/publications/detail/sp/800-218/final)

