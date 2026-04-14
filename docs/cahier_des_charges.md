# Cahier des Charges - Pipeline CI/CD Sécurisé DevSecOps

## 📋 Document de Spécification

**Projet:** E-Commerce Platform - Sécurisation Supply Chain Logicielle  
**Sujet:** Mise en place d'un pipeline CI/CD sécurisé avec GitHub Actions  
**Version:** 2.0  
**Date:** 14 Avril 2026  
**Consultant:** M. Bonitah RAMBELOSON (DevOps | Cloud Engineer | MLOps Practitioner)

---

## 1. Objectifs Globaux

### Objectif Principal
Concevoir, implémenter et démontrer une **chaîne CI/CD sécurisée** assurant :
- La construction automatisée d'images Docker
- La vérification de sécurité du code source
- La détection des secrets exposés
- Le scan des vulnérabilités (code, dépendances, conteneurs)
- Le déploiement contrôlé et tracé

### Objectifs Secondaires
✅ Automatiser les tests et la qualité du code  
✅ Détecter les vulnérabilités précocement (shift-left security)  
✅ Garantir la conformité **OWASP Top 10** et **CWE Top 25**  
✅ Générer un **SBOM** (Software Bill of Materials)  
✅ Documenter l'architecture sécurisée  
✅ Assurer la traçabilité complète des artefacts  

---

## 2. Contexte et Justification

Les attaques sur la supply chain logicielle (ex. SolarWinds, Log4j, npm packages compromis) ont montré la nécessité de sécuriser les pipelines CI/CD.

**Ce projet démontre les principes DevSecOps** :
- 🔐 **Security-by-Design** : Sécurité dès la conception
- 🔄 **Continuous Security** : Vérifications à chaque étape
- 🤖 **Automation First** : Zero-trust, automation obligatoire
- 📊 **Visibility & Auditability** : Logs complets et traçables

---

## 3. Périmètre du Projet

### ✅ Composants In Scope
- **Backend:** API .NET 8.0 (EcomApi) - C#
- **Frontend:** React 19 + Vite 6 - JavaScript/TypeScript
- **CI/CD:** GitHub Actions (workflows automatisés)
- **Registre:** GitHub Container Registry (GHCR)
- **Scans de sécurité:**
  - 🔐 Gitleaks (détection secrets)
  - 🛡️ Semgrep (analyse statique SAST)
  - 📦 OWASP Dependency-Check (vulnérabilités dépendances)
  - 🐳 Trivy (scan images Docker)
  - 📋 Anchore SBOM (Bill of Materials)
  - ESLint (qualité code frontend)
  - dotnet test (tests backend)

### ❌ Composants Out of Scope
- Déploiement en production cloud
- Infrastructure Kubernetes
- Harbor Registry (remplacé par GHCR)
- Monitoring Prometheus/Grafana
- Vault pour secrets dynamiques

---

## 4. Architecture du Pipeline CI/CD

### Diagramme Simplifié
```
Developer Push → GitHub Repository
    ↓
GitHub Actions Auto-Trigger
    ├─ 🔐 Secret Scan (Gitleaks)
    ├─ 🛡️ SAST (Semgrep)
    ├─ 📦 Dependency Check (OWASP DC, npm audit)
    ├─ 🔨 Build (dotnet, npm)
    ├─ 🧪 Tests & Lint
    ├─ 🐳 Container Scan (Trivy)
    └─ ✅ Quality Gate
         ↓
    GitHub Artifacts & GHCR Registry
```

---

## 5. Workflows Détaillés

### Workflow Backend (`ci-backend.yml`)
| Stage | Outil | Détails |
|-------|-------|---------|
| 🔐 Secrets | Gitleaks | Détecte API keys, tokens, credentials |
| 🛡️ SAST | Semgrep | OWASP Top 10, CWE Top 25 |
| 📦 Dépend. | OWASP DC | Scan CVE publiques NuGet |
| 🔨 Build | dotnet | Compilation Release |
| 🧪 Tests | dotnet test | Unit tests & coverage |
| 🐳 Container | Trivy | Scan vulnerabilités Docker |
| ✅ Gate | GitHub | Validation finale |

**Triggers:** Push/PR sur `main`, `develop` + paths `backend/**`

### Workflow Frontend (`ci-frontend.yml`)
| Stage | Outil | Détails |
|-------|-------|---------|
| 🔐 Secrets | Gitleaks | Détecte secrets en JS/TS |
| 🛡️ SAST | Semgrep | OWASP Top 10, JavaScript rules |
| 📦 Dépend. | npm audit | Audit packages npm |
| 📋 SBOM | Anchore | Software Bill of Materials |
| �� Build | Vite | npm run build |
| 🧪 Lint | ESLint | Code quality |
| 🐳 Container | Trivy | Scan image |
| ✅ Gate | GitHub | Validation |

**Triggers:** Push/PR sur `main`, `develop` + paths `frontend/**`

---

## 6. Stack Technologique

| Catégorie | Technologie | Version |
|-----------|-------------|---------|
| **Backend** | .NET SDK | 8.0.x |
| **Frontend** | React + Vite | 19.0 + 6.2 |
| **Database** | PostgreSQL | (Npgsql 8.0.2) |
| **Auth** | JWT Bearer | 8.0.2 |
| **UI** | Material-UI + Tailwind | 9.0 + 4.0 |
| **State** | Redux Toolkit | 2.7.0 |
| **Secrets** | Gitleaks | v0.18+ |
| **SAST** | Semgrep | v1.45+ |
| **Dépend.** | OWASP DC + npm audit | Latest |
| **Container** | Trivy | v0.47+ |
| **SBOM** | Anchore SBOM | v0.15+ |

---

## 7. Critères d'Acceptation

### 🔐 Security Criteria
- ✅ **0 secret exposé** accepté
- ✅ **0 vulnérabilité CRITIQUE** en main branch
- ✅ **OWASP coverage > 90%**
- ✅ **CWE coverage > 80%**
- ✅ **SBOM** généré automatiquement
- ✅ **Trivy scan** sans erreurs

### 📊 Quality Criteria
- ✅ **Tests coverage > 80%** (backend)
- ✅ **ESLint: 0 errors** (frontend)
- ✅ **Pipeline duration < 30 min** (backend)
- ✅ **Pipeline duration < 20 min** (frontend)

### 🎯 Compliance Criteria
- ✅ **OWASP Top 10** compliance
- ✅ **CWE Top 25** coverage
- ✅ **DevSecOps principles** appliquées
- ✅ **Audit trail** complet

---

## 8. Risques et Mitigations

| Risque | Probabilité | Mitigation |
|--------|-------------|-----------|
| Secrets exposés | 🔴 Haute | Gitleaks + pre-commit hooks |
| Dépendances vulnérables | 🟡 Moyenne | npm audit + OWASP DC + pinning |
| Images Docker compromises | 🟢 Basse | Trivy + minimal base images |
| Build failures | �� Moyenne | Retry logic + caching |
| Branch bypass | 🟢 Basse | GitHub branch protection rules |

---

## 9. Livrables Attendus

### 📁 Code et Configuration
✅ `.github/workflows/ci-backend.yml`  
✅ `.github/workflows/ci-frontend.yml`  
✅ `backend/Dockerfile` & `frontend/Dockerfile`  
✅ `docker-compose.yml` (optionnel)  

### 📚 Documentation
✅ `docs/cahier_des_charges.md`  
✅ `docs/analyse_risques.md`  
✅ `docs/rapport_final.md`  

### 🔐 Configuration Sécurité
✅ `.gitignore` complet  
✅ GitHub secrets configurés  
✅ Branch protection rules  

---

## 10. Timeline

| Phase | Durée | Status |
|-------|-------|--------|
| Phase 1: Setup GitHub Actions | Semaines 1-2 | ✅ Complète |
| Phase 2: Intégration scans | Semaines 3-4 | ✅ Complète |
| Phase 3: Dependency & Container | Semaines 5-6 | ✅ Complète |
| Phase 4: Tests & Validation | Semaines 7-8 | 🟡 En cours |
| Phase 5: Documentation | Semaines 9-10 | 🟡 En cours |
| Phase 6: Audit & Démo | Semaines 11-12 | ⏳ À venir |

---

## 11. Success Metrics

- ✅ Pipeline success rate > 95%
- ✅ Secrets detection: 100%
- ✅ MTTR (Mean Time To Remediate) < 24h
- ✅ Code coverage > 80%
- ✅ 0 CRITICAL findings en production

---

## 12. Conformité OWASP & CWE

### OWASP Top 10 (2021)
- A01 Broken Access Control → Branch protection
- A02 Cryptographic Failures → Secrets scanning
- A05 Injection → SAST (Semgrep)
- A06 Vulnerable Components → Dependency check
- A10 Supply Chain → SBOM + traceability

### CWE Top 25
- CWE-79 (XSS) → Semgrep rules
- CWE-89 (SQL Injection) → Semgrep rules
- CWE-798 (Hardcoded Credentials) → Gitleaks

---

**Version:** 2.0 | **Dernière MAJ:** 14 Avril 2026
