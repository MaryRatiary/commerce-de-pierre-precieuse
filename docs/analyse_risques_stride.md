# Analyse des Risques STRIDE - Pipeline CI/CD

## 📊 Méthodologie STRIDE

**STRIDE** = Spoofing, Tampering, Repudiation, Information Disclosure, Denial of Service, Elevation of Privilege

---

## 1. SPOOFING (Usurpation d'Identité)

### Risque 1.1: Usurpation d'identité du développeur
- **Description:** Attaquant pousse du code avec identité fausse
- **Impact:** Malware introduit dans la supply chain
- **Probabilité:** Moyenne | **Sévérité:** Critique
- **Mitigation:**
  - ✅ Authentification GitHub 2FA obligatoire
  - ✅ Signature des commits GPG (recommandé)
  - ✅ Protection des branches (require review)
  - ✅ Audit logs des commits

### Risque 1.2: Usurpation du pipeline CI/CD
- **Description:** Modification du workflow GitHub Actions
- **Impact:** Injection de code malveillant
- **Probabilité:** Basse | **Sévérité:** Critique
- **Mitigation:**
  - ✅ Branch protection rules
  - ✅ Secrets management via GitHub Secrets
  - ✅ Audit des modifications workflow

---

## 2. TAMPERING (Altération)

### Risque 2.1: Altération du code source
- **Description:** Injection de vulnérabilités lors du push
- **Impact:** Vulnérabilités en production
- **Probabilité:** Moyenne | **Sévérité:** Critique
- **Mitigation:**
  - ✅ Scans SAST (Semgrep) obligatoires
  - ✅ Pull requests avec reviews automatiques
  - ✅ Status checks dans GitHub
  - ✅ Semantic versioning

### Risque 2.2: Altération des artefacts de build
- **Description:** Modification des artifacts générés
- **Impact:** Distribution de malware
- **Probabilité:** Basse | **Sévérité:** Critique
- **Mitigation:**
  - ✅ Checksums/Hashes des artifacts
  - ✅ Signature des images Docker (Cosign)
  - ✅ Audit trail GitHub Actions
  - ✅ Immutable artifacts

### Risque 2.3: Altération des dépendances
- **Description:** Dépendances malveillantes ou compromises
- **Impact:** Supply chain attack
- **Probabilité:** Moyenne | **Sévérité:** Haute
- **Mitigation:**
  - ✅ npm audit / dotnet list --vulnerable
  - ✅ Dependency pinning avec checksums
  - ✅ SBOM generation (Syft)
  - ✅ Notifications vulnérabilités

---

## 3. REPUDIATION (Répudiation)

### Risque 3.1: Denial of responsability
- **Description:** Développeur nie une action
- **Impact:** Perte de traçabilité
- **Probabilité:** Basse | **Sévérité:** Moyenne
- **Mitigation:**
  - ✅ Audit logs complets GitHub
  - ✅ Logs GitHub Actions immuables
  - ✅ Commits signés GPG
  - ✅ Webhook notifications Slack

---

## 4. INFORMATION DISCLOSURE (Divulgation d'Info)

### Risque 4.1: Secrets exposés en clair
- **Description:** API keys, tokens dans le code
- **Impact:** Compromission des comptes
- **Probabilité:** Haute | **Sévérité:** Critique
- **Mitigation:**
  - ✅ Gitleaks scan sur tous les commits
  - ✅ Pre-commit hooks local
  - ✅ GitHub Secrets pour variables sensibles
  - ✅ Rotation régulière des secrets

### Risque 4.2: Logs sensibles exposés
- **Description:** Information sensible dans les logs CI/CD
- **Impact:** Divulgation d'architecture
- **Probabilité:** Moyenne | **Sévérité:** Moyenne
- **Mitigation:**
  - ✅ Masquage automatique secrets en logs
  - ✅ Accès restreint aux logs (GitHub)
  - ✅ Rotation des credentials
  - ✅ Audit logs access

### Risque 4.3: Container registry compromise
- **Description:** Images Docker compromises
- **Impact:** Déploiement de malware
- **Probabilité:** Basse | **Sévérité:** Critique
- **Mitigation:**
  - ✅ Trivy scan avant push
  - ✅ Signature images (Cosign/Notary)
  - ✅ Private registry (ghcr.io)
  - ✅ Image scanning policy

---

## 5. DENIAL OF SERVICE (Déni de Service)

### Risque 5.1: Pipeline DoS
- **Description:** Attaquant force pipeline à s'exécuter infiniment
- **Impact:** Coûts élevés, délais de deploy
- **Probabilité:** Basse | **Sévérité:** Moyenne
- **Mitigation:**
  - ✅ Timeout sur workflows (1 heure max)
  - ✅ Concurrency limits
  - ✅ Build cache optimization
  - ✅ Rate limiting actions

### Risque 5.2: Dependency confusion attack
- **Description:** Package malveillant avec même nom
- **Impact:** Installation package malveillant
- **Probabilité:** Basse | **Sévérité:** Haute
- **Mitigation:**
  - ✅ Lock files (package-lock.json)
  - ✅ Private npm scope (@company)
  - ✅ Dependency scanning
  - ✅ Explicit version pinning

---

## 6. ELEVATION OF PRIVILEGE

### Risque 6.1: Escalade privs dans artifacts
- **Description:** Vulnérabilité dans dépendance
- **Impact:** Exécution code arbitraire
- **Probabilité:** Moyenne | **Sévérité:** Critique
- **Mitigation:**
  - ✅ OWASP Dependency Check
  - ✅ CVE scanning automatique
  - ✅ Base images hardened
  - ✅ Principle of least privilege

### Risque 6.2: Compromission GitHub Actions Runner
- **Description:** Runner compromis
- **Impact:** Accès complet au repo et artifacts
- **Probabilité:** Basse | **Sévérité:** Critique
- **Mitigation:**
  - ✅ GitHub-hosted runners (recommended)
  - ✅ Self-hosted runners: isolation réseau
  - ✅ Ephemeral runners
  - ✅ Audit des actions utilisées

---

## Matrice de Risques

| Risque | Probabilité | Sévérité | Score | État |
|--------|-------------|----------|-------|------|
| Secrets exposés | 🔴 Haute | 🔴 Critique | 25 | Mitigé |
| Usurpation identité dev | 🟡 Moyenne | 🔴 Critique | 20 | Mitigé |
| Altération code | 🟡 Moyenne | 🔴 Critique | 20 | Mitigé |
| Dépendances malveilles | 🟡 Moyenne | 🟠 Haute | 15 | Mitigé |
| Images Docker compromises | 🟢 Basse | 🔴 Critique | 10 | Mitigé |
| Pipeline DoS | 🟢 Basse | 🟡 Moyenne | 5 | Acceptable |

---

## Recommendations

### Court terme (Immédiat)
1. ✅ Activer 2FA sur tous les comptes GitHub
2. ✅ Configurer secrets GitHub
3. ✅ Gitleaks scan obligatoire
4. ✅ Branch protection rules

### Moyen terme (1-2 mois)
1. ✅ Signature commits GPG
2. ✅ Signature images Docker
3. ✅ SBOM automatique
4. ✅ Scanning dépendances

### Long terme (3-6 mois)
1. ✅ Vault pour secret management
2. ✅ Policy-as-Code (OPA)
3. ✅ Monitoring & alerting
4. ✅ Incident response automation

---

## Compliance

✅ **OWASP Top 10:**
- A01:2021 – Broken Access Control → Branch protection
- A02:2021 – Cryptographic Failures → Secret scanning
- A05:2021 – Injection → SAST scanning
- A06:2021 – Vulnerable Components → Dependency check
- A10:2021 – Supply Chain → SBOM, signatures

✅ **CWE Top 25:**
- CWE-79 (XSS) → Semgrep rules
- CWE-89 (SQL Injection) → Semgrep rules
- CWE-295 (Cert Validation) → Container scan
- CWE-798 (Hardcoded Credentials) → Gitleaks

