# Cahier des Charges - Projet CI/CD Sécurisé

## 📋 Document de Spécification

**Projet:** E-Commerce Platform - Sécurisation Supply Chain  
**Version:** 1.0  
**Date:** Avril 2026  
**Auteur:** Equipe DevSecOps  

---

## 1. Objectifs du Projet

### Objectif Principal
Mettre en place une chaîne CI/CD automatisée et sécurisée pour l'application e-commerce utilisant :
- **GitHub** pour la gestion du code source
- **GitHub Actions** pour l'orchestration CI/CD (alternative à Jenkins)
- **Scans de sécurité** intégrés (SAST, dépendances, secrets)
- **Artefacts sécurisés** (build et tests validés)

### Objectifs Secondaires
1. ✅ Automatiser les tests et la qualité du code
2. ✅ Détecter les vulnérabilités précocement
3. ✅ Garantir la conformité OWASP et CWE
4. ✅ Générer un rapport SBOM (Software Bill of Materials)
5. ✅ Documenter l'architecture sécurisée

---

## 2. Périmètre du Projet

### Composants In Scope
- **Backend:** API .NET 8.0 (EcomApi)
- **Frontend:** React + Vite
- **Orchestration:** GitHub Actions + Jenkins (configuration locale optionnelle)
- **Scans:** Gitleaks, Semgrep, Trivy, npm audit, dotnet list package

### Composants Out of Scope
- Déploiement en production cloud
- Infrastructure Kubernetes
- Registry privée Harbor (remplacé par GitHub Container Registry)

---

## 3. Architecture CI/CD

```
Developer Push → GitHub Repository
    ↓
GitHub Actions Triggers
    ├─→ [Secrets Detection] (Gitleaks)
    ├─→ [SAST Analysis] (Semgrep)
    ├─→ [Dependency Scan] (npm audit, dotnet list)
    ├─→ [Build Backend] (.NET Release)
    ├─→ [Build Frontend] (React/Vite)
    ├─→ [Container Scan] (Trivy)
    ├─→ [Quality Gate] (SonarQube)
    └─→ [Generate Reports] (SBOM, SARIF)
         ↓
    Artifacts Uploaded to GitHub
    ↓
Jenkins (Trigger Manual - Optional)
    └─→ Full Pipeline Validation
```

---

## 4. Workflows et Stages

### Workflow Backend (`ci-backend.yml`)
| Stage | Outil | Description |
|-------|-------|-------------|
| Secret Scan | Gitleaks | Détecte secrets, API keys, tokens |
| SAST | Semgrep | Analyse statique (OWASP Top 10, CWE) |
| Dependency | OWASP DC | Scan vulnérabilités dépendances NuGet |
| Build | dotnet | Compilation en configuration Release |
| Tests | dotnet test | Exécution tests unitaires |
| Container Scan | Trivy | Scan vulnérabilités images Docker |
| Quality Gate | SonarQube | Validation métriques qualité |

### Workflow Frontend (`ci-frontend.yml`)
| Stage | Outil | Description |
|-------|-------|-------------|
| Secret Scan | Gitleaks | Détecte secrets |
| SAST | Semgrep | Analyse JavaScript/TypeScript |
| Dependency | npm audit | Audit packages npm |
| SBOM | Syft/Anchore | Génère composants logiciels |
| Build | npm run build | Build Vite production |
| Container Scan | Trivy | Scan image Docker frontend |
| Quality Gate | ESLint | Validation code quality |

---

## 5. Critères d'Acceptation

### Security
- ✅ 0 secret exposé détecté
- ✅ 0 vulnérabilité CRITIQUE acceptée
- ✅ 0 vulnérabilité HAUTE non documentée
- ✅ SBOM généré et stocké

### Quality
- ✅ Coverage de tests > 80%
- ✅ Code smell score < 5%
- ✅ Tous les lints passent (ESLint, dotnet format)

### Performance
- ✅ Pipeline < 30 min (backend)
- ✅ Pipeline < 20 min (frontend)
- ✅ Build artifacts < 500MB

---

## 6. Risques et Mitigations

| Risque | Impact | Mitigation |
|--------|--------|-----------|
| Secrets exposés | Critique | Gitleaks scan, gitignore, secret manager |
| Dépendances vulnérables | Haute | Scans automatiques, pinning versions |
| Conteneurs malveillants | Haute | Trivy scan, images de base mises à jour |
| Build failures | Moyenne | Retry logic, cache optimization |

---

## 7. Livrables

1. ✅ `.github/workflows/ci-backend.yml` - Pipeline backend automatisé
2. ✅ `.github/workflows/ci-frontend.yml` - Pipeline frontend automatisé
3. ✅ `.jenkins/Jenkinsfile` - Pipeline Jenkins alternatif
4. ✅ `.jenkins/scripts/` - Scripts de sécurité réutilisables
5. ✅ `docs/` - Documentation complète
6. ✅ `security/` - Policies et configurations sécurité
7. ✅ Rapports SBOM et vulnérabilités

---

## 8. Timeline

| Phase | Durée | Activités |
|-------|-------|-----------|
| Phase 1 | Semaine 1 | Setup GitHub Actions, configuration initiale |
| Phase 2 | Semaine 2-3 | Intégration scans sécurité |
| Phase 3 | Semaine 4 | Tests et validation |
| Phase 4 | Semaine 5 | Documentation et démonstration |

---

## 9. Success Metrics

- ✅ Pipeline s'exécute à 100% des pushs
- ✅ 0 secret exposé en production
- ✅ Temps build < 30 min
- ✅ Couverture de tests > 80%
- ✅ Rapport SBOM généré automatiquement

