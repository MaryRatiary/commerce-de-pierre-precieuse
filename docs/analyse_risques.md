# Analyse des Risques - STRIDE

## 📊 Analyse STRIDE du Pipeline CI/CD Sécurisé

**Projet:** E-Commerce Platform - Pipeline CI/CD DevSecOps  
**Date:** 14 Avril 2026  
**Méthodologie:** STRIDE (Spoofing, Tampering, Repudiation, Information Disclosure, Denial of Service, Elevation of Privilege)

---

## 1. Vue d'ensemble STRIDE

```
┌─────────────────────────────────────────────────────────────┐
│ STRIDE Risk Analysis - Supply Chain Security                │
├─────────────────────────────────────────────────────────────┤
│ S - Spoofing:                User/System identity spoofing  │
│ T - Tampering:               Code/Artifact modification     │
│ R - Repudiation:             Denial of actions              │
│ I - Information Disclosure:  Secret exposure                │
│ D - Denial of Service:       Pipeline unavailability        │
│ E - Elevation of Privilege:  Unauthorized access escalation │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Spoofing (S) - Usurpation d'Identité

### 2.1 Actor Spoofing - GitHub Token Theft

**Menace:** Un attaquant obtient un token GitHub et simule un développeur

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🔴 Moyenne |
| **Sévérité** | 🔴 Critique |
| **Impact** | Modification de code, injection malveillante |

**Vecteurs d'attaque:**
- Token exposé dans les logs
- Token hardcoded dans `.env`
- Token compromis via phishing

**Mitigations Actuelles:**
✅ GitHub secrets management (masked in logs)  
✅ Branch protection rules (require reviews)  
✅ Gitleaks détecte tokens exposés  

**Mitigations Recommandées:**
🔵 Activer **branch protection** avec code review obligatoire  
🔵 Utiliser **OIDC** au lieu de tokens classiques (GitHub native)  
🔵 Rotation des secrets tous les 90 jours  
🔵 Audit logs GitHub mensuels  

---

### 2.2 System Spoofing - GitHub Actions Compromise

**Menace:** Les workflows GitHub Actions sont modifiés par un attaquant

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟢 Basse |
| **Sévérité** | 🔴 Critique |
| **Impact** | Pipeline malveillant, exfiltration de secrets |

**Vecteurs d'attaque:**
- Modification de `.github/workflows/*.yml`
- Injection dans des actions tiers compromises
- Workflow artifact poisoning

**Mitigations Actuelles:**
✅ Version pinning des GitHub Actions (ex: `@v4`)  
✅ Code review sur tous les commits  
✅ Branch protection sur main  

**Mitigations Recommandées:**
🔵 Utiliser **GitHub Actions** officielles uniquement  
🔵 Implémenter **CODEOWNERS** (require approval)  
🔵 Audit les actions utilisées trimestriellement  
🔵 Policy-as-Code (OPA/Conftest) sur workflows  

---

## 3. Tampering (T) - Modification Non Autorisée

### 3.1 Code Tampering - Injection de Malveillance

**Menace:** Un attaquant modifie le code source via pull request

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟡 Moyenne |
| **Sévérité** | 🔴 Critique |
| **Impact** | Code malveillant en production |

**Vecteurs d'attaque:**
- Pull request avec code injecté
- Supply chain attack (dépendance malveillante)
- Typosquatting npm packages

**Mitigations Actuelles:**
✅ Semgrep SAST détecte patterns malveillants  
✅ Code review obligatoire (branch protection)  
✅ npm audit détecte dépendances malveillantes  
✅ OWASP Dependency-Check scan CVE  

**Mitigations Recommandées:**
🔵 **Require multiple approvals** pour main branch  
🔵 Intégrer **SBOM signing** (Syft)  
🔵 Dépendances **pinned** (lock files)  
�� Whitelist de packages npm approuvés (Verdaccio)  
🔵 Supply chain risk assessment trimestriel  

---

### 3.2 Artifact Tampering - Modification d'Images Docker

**Menace:** Images Docker modifiées avant déploiement

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟢 Basse |
| **Sévérité** | 🔴 Critique |
| **Impact** | Container compromis en production |

**Vecteurs d'attaque:**
- GHCR registry compromise
- Man-in-the-middle sur push/pull Docker
- Layer poisoning dans image

**Mitigations Actuelles:**
✅ Images scannées avec Trivy (vulnérabilités)  
✅ GHCR authentification (GitHub token)  
✅ Minimal base images (Alpine)  

**Mitigations Recommandées:**
🔵 **Image signing** (Cosign/Notary v2)  
🔵 **Image verification** avant déploiement  
🔵 **Immutable tags** (commit SHA)  
�� Registry scanning policies (GHCR/Harbor)  
🔵 Supply chain security framework (SLSA)  

---

### 3.3 Test Tampering - Contournement des Tests

**Menace:** Tests désactivés ou modifiés pour masquer vulnérabilités

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟡 Moyenne |
| **Sévérité** | 🟠 Haute |
| **Impact** | Vulnérabilités non détectées |

**Vecteurs d'attaque:**
- Tests supprimés ou skippés
- Coverage reporting falsifié
- Mock injections malveillantes

**Mitigations Actuelles:**
✅ Tests obligatoires dans pipeline  
✅ Code review sur modifications tests  
✅ ESLint et dotnet test runs  

**Mitigations Recommandées:**
🔵 **Minimum coverage enforced** (80%+)  
🔵 **Fail-on-warning** pour tests  
🔵 Test audit logs  
🔵 Separate test approval reviewer  

---

## 4. Repudiation (R) - Déni d'Actions

### 4.1 Log Tampering - Modification des Logs Pipeline

**Menace:** Un attaquant efface les logs pour masquer ses actions

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟢 Basse |
| **Sévérité** | 🟠 Haute |
| **Impact** | Perte de traçabilité, investigation impossible |

**Vecteurs d'attaque:**
- Accès GitHub runner compromis
- Suppression logs artifactsGitHub
- Modification audit logs

**Mitigations Actuelles:**
✅ GitHub logs immutables (30 jours rétention)  
✅ Workflow run history sauvegardé  
✅ Artifact logs (.github/workflows/)  

**Mitigations Recommandées:**
🔵 **SIEM integration** (log export externe)  
🔵 **Log signing** (tamper detection)  
🔵 **Extended retention** (6+ mois)  
🔵 **Immutable logging** (append-only)  
🔵 Audit logs GitHub mensuel review  

---

### 4.2 Deployment Denial - Déni de Déploiements

**Menace:** Un utilisateur nie avoir déployé du code malveillant

| Aspect | Détail |
|--------|--------|
| **Probabilité** | �� Basse |
| **Sévérité** | 🟠 Haute |
| **Impact** | Non-répudiation compromise |

**Vecteurs d'attaque:**
- Credentials partagés (plusieurs users)
- Suppression commit history
- Forge de timestamps

**Mitigations Actuelles:**
✅ GitHub commit signing (GPG)  
✅ User-tied PAT (Personal Access Tokens)  
✅ Audit trail complet  

**Mitigations Recommandées:**
🔵 **Require commit signing** (main branch)  
🔵 **Git log audit** mensuel  
🔵 **User PAT expiration** (90 jours)  
🔵 **Deployment approval records**  
🔵 **Non-repudiation certificates** (PKI)  

---

## 5. Information Disclosure (I) - Exposition de Secrets

### 5.1 Secrets Exposure - Hardcoded Credentials

**Menace:** Secrets (API keys, passwords, tokens) exposés dans le code

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🔴 Haute |
| **Sévérité** | 🔴 Critique |
| **Impact** | Compromise de tous les systèmes connectés |

**Vecteurs d'attaque:**
- API keys en dur dans code
- Passwords dans comments
- Database credentials en .env publique

**Mitigations Actuelles:**
✅ **Gitleaks scanning** (détecte 100+ patterns)  
✅ GitHub secrets management (masked in logs)  
✅ `.gitignore` pour .env files  

**Mitigations Recommandées:**
🔵 **Pre-commit hooks** (local Gitleaks)  
🔵 **Secret rotation** (90 jours max)  
🔵 **Vault integration** (dynamique secrets)  
🔵 **Key scanning** (private key detection)  
🔵 **Audit secrets usage** mensuellement  

**STAT:** 🟢 Gitleaks a détecté **0 secret** en 2026  

---

### 5.2 Build Artifacts Exposure - Artifacts Visibles

**Menace:** Build artifacts contiennent des secrets ou données sensibles

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟡 Moyenne |
| **Sévérité** | 🟠 Haute |
| **Impact** | Exposition de paths, versions, configs |

**Vecteurs d'attaque:**
- Artifacts publiquement accessibles
- Debug symbols exposés
- Config files dans artifacts

**Mitigations Actuelles:**
✅ GitHub artifacts avec retention limitée  
✅ Private GHCR registry (authentification)  
✅ Artifacts auto-deleted après 30 jours  

**Mitigations Recommandées:**
🔵 **Artifact encryption** (AES-256)  
🔵 **Signed artifacts** (Cosign)  
🔵 **Artifact audit log**  
🔵 **Retention policy enforcement**  
🔵 **Immutable artifact IDs**  

---

### 5.3 Configuration Exposure - CI/CD Settings Leak

**Menace:** Configuration sensible exposée (API endpoints, database hosts)

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟡 Moyenne |
| **Sévérité** | 🟠 Haute |
| **Impact** | Information gathering pour attaque future |

**Vecteurs d'attaque:**
- Logs contenant endpoints API
- Docker build args exposés
- Configuration files en artifacts

**Mitigations Actuelles:**
✅ GitHub secrets (masked in logs)  
✅ Sensitive logs filtered  
✅ Private environment variables  

**Mitigations Recommandées:**
🔵 **Redact sensitive values** from logs  
🔵 **Config Server** (centralized, encrypted)  
🔵 **Log sanitization** (regex patterns)  
🔵 **Audit log access**  

---

## 6. Denial of Service (D) - Indisponibilité

### 6.1 Pipeline DoS - Resource Exhaustion

**Menace:** Attaquant lance des pipelines coûteuses pour épuiser les ressources

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟡 Moyenne |
| **Sévérité** | 🟡 Moyenne |
| **Impact** | Pipeline lenteur, retard déploiement |

**Vecteurs d'attaque:**
- Commits massifs (large files)
- Infinite loops dans tests
- Excessive docker builds

**Mitigations Actuelles:**
✅ Path triggers (only `backend/`, `frontend/`)  
✅ Parallel job limits  
✅ Timeout enforcement  

**Mitigations Recommandées:**
🔵 **Quota enforcement** (GitHub Actions minutes)  
🔵 **Rate limiting** sur commits/PRs  
🔵 **Resource limits** (memory, CPU)  
🔵 **Cost monitoring** (GitHub Actions usage)  
🔵 **Concurrent run limits**  

---

### 6.2 Registry DoS - GHCR Unavailability

**Menace:** GHCR registry DoS empêche pulls d'images

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟢 Basse |
| **Sévérité** | 🟠 Haute |
| **Impact** | Déploiement impossible |

**Vecteurs d'attaque:**
- Malicious image uploads (large layers)
- Registry bandwidth saturation
- GitHub API rate limiting

**Mitigations Actuelles:**
✅ GHCR managed by GitHub (SLA 99.9%)  
✅ Automatic rate limiting  
✅ CDN caching layers  

**Mitigations Recommandées:**
🔵 **Local registry cache** (pull-through proxy)  
🔵 **Image mirror** (secondary registry)  
🔵 **Bandwidth monitoring**  
🔵 **DDoS protection** (Cloudflare, WAF)  

---

## 7. Elevation of Privilege (E) - Escalade de Privilèges

### 7.1 Workflow Permission Escalation

**Menace:** Workflow avec permissions excessives (e.g., `contents: write`)

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟡 Moyenne |
| **Sévérité** | 🔴 Critique |
| **Impact** | Write access to repo, secrets exposure |

**Vecteurs d'attaque:**
- Workflow avec `contents: write` sur tous les jobs
- Secrets exposed à des actions non-trustées
- Workflow file modification access

**Mitigations Actuelles:**
✅ Least privilege permissions (read-only default)  
✅ Specific job permissions  
✅ GitHub token scoped  

**Mitigations Recommandées:**
🔵 **Permission audit** (trimestriel)  
🔵 **Enforce read-only** (default)  
🔵 **Policy-as-Code** (Conftest)  
🔵 **Role separation** (build ≠ deploy)  
�� **Periodic access review**  

---

### 7.2 Action Dependency Escalation

**Menace:** GitHub Actions malveillantes obtiennent accès aux secrets

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟡 Moyenne |
| **Sévérité** | 🔴 Critique |
| **Impact** | Secrets extraction, code injection |

**Vecteurs d'attaque:**
- Malicious action dans marketplace
- Compromised official action
- Typosquatting action names

**Mitigations Actuelles:**
✅ Only official GitHub Actions (verified)  
✅ Version pinning (e.g., `@v4`)  
✅ Reviewed before use  

**Mitigations Recommandées:**
🔵 **Action approval list** (whitelist)  
🔵 **Action audit log**  
🔵 **Self-hosted runners** (isolated)  
🔵 **Action source inspection** (quarterly)  
🔵 **Sandbox execution** (container isolation)  

---

### 7.3 Runner Privilege Escalation

**Menace:** GitHub Actions runner (Ubuntu) compromise → full system access

| Aspect | Détail |
|--------|--------|
| **Probabilité** | 🟢 Basse |
| **Sévérité** | 🔴 Critique |
| **Impact** | Complete system compromise |

**Vecteurs d'attaque:**
- Kernel exploit sur Ubuntu runner
- Docker escape
- Privilege escalation via sudo

**Mitigations Actuelles:**
✅ GitHub-hosted runners (patched, ephemeral)  
✅ No privileged container access  
✅ Isolated job environment  

**Mitigations Recommandées:**
🔵 **Security updates** (weekly)  
🔵 **Kernel hardening** (AppArmor, seccomp)  
🔵 **Self-hosted runners in VPC** (isolated)  
🔵 **Container image scanning** (base OS)  
🔵 **Runtime monitoring** (Falco)  

---

## 8. Résumé des Risques - Risk Matrix

### Heat Map - Impact vs Probabilité

```
┌─────────────────────────────────────────────────────────┐
│ SEVERITY / PROBABILITY MATRIX                           │
├─────────────────────────────────────────────────────────┤
│                         PROBABILITÉ                     │
│                Basse    Moyenne    Haute                │
│ Sé │ CRITIQUE   (S)   (T,I,E)    (I-Secrets)            │
│ vé │ HAUTE      (D)   (T,I,E)    (S-Token,I)            │
│ ri │ MOYENNE    (R)   (T,D)      -                      │
│ té │                                                    │
└─────────────────────────────────────────────────────────┘

Legende:
S = Spoofing    T = Tampering    R = Repudiation
I = Information Disclosure    D = Denial of Service
E = Elevation of Privilege
```

### Top 5 Risques Prioritaires

| Rang | Risque | Probabilité | Sévérité | Score |
|------|--------|-------------|----------|-------|
| 🔴 1 | Secrets exposure (hardcoded) | Haute | Critique | 9.5 |
| 🔴 2 | Actor spoofing (token theft) | Moyenne | Critique | 8.5 |
| 🔴 3 | Code tampering (malware injection) | Moyenne | Critique | 8.5 |
| 🔴 4 | Artifact tampering (Docker images) | Basse | Critique | 7.0 |
| 🟠 5 | Workflow privilege escalation | Moyenne | Critique | 8.0 |

---

## 9. Plan de Mitigation - Timeline

### Court Terme (30 jours)
- ✅ Gitleaks scanning en place
- ✅ Branch protection configurée
- ✅ Semgrep SAST intégré
- 🟡 Pre-commit hooks (TO-DO)
- 🟡 Secret rotation policy (TO-DO)

### Moyen Terme (90 jours)
- 🔵 Image signing (Cosign)
- 🔵 SBOM generation & verification
- 🔵 Supply chain policy enforcement
- 🔵 Log signing & immutability
- 🔵 Action whitelist

### Long Terme (6+ mois)
- 🔵 Vault integration (dynamique secrets)
- 🔵 Policy-as-Code (OPA/Conftest)
- 🔵 SLSA framework implementation
- 🔵 Monitoring sécurité (Prometheus)
- 🔵 Supply chain risk assessment

---

## 10. Métriques et KPIs de Sécurité

| Métrique | Cible | Actuel | Status |
|----------|-------|--------|--------|
| Secrets détectés | 0/commit | 0 | ✅ |
| Vulnerabilités CRITIQUE | 0 | 0 | ✅ |
| OWASP coverage | > 90% | ~85% | 🟡 |
| CWE coverage | > 80% | ~80% | ✅ |
| SBOM generation | 100% | En cours | 🟡 |
| Image signing | 100% | 0% | 🔴 |
| Mean Time to Fix | < 24h | N/A | ⏳ |
| Audit trail retention | 6+ months | 30 days | 🟡 |

---

## 11. Recommandations Prioritaires

### 🔴 CRITICAL (Implémenter immédiatement)
1. **Secrets Scanning Local** - Pre-commit hooks Gitleaks
2. **Image Signing** - Cosign pour toutes les images
3. **Permission Audit** - Review tous les workflows
4. **Branch Rules** - Require multiple reviews (main)

### 🟠 HIGH (Implémenter ce trimestre)
1. **Supply Chain Policy** - SLSA framework
2. **Log Signing** - Immutability & tamper detection
3. **Secret Rotation** - 90-day policy
4. **Action Whitelist** - Approved actions only

### 🟡 MEDIUM (Implémenter cette année)
1. **Vault Integration** - Dynamic secrets
2. **Policy-as-Code** - OPA/Conftest
3. **SIEM Integration** - External log export
4. **Monitoring** - Prometheus + Grafana

---

## 12. Conclusion

Le pipeline GitHub Actions actuel implémente **un bon niveau de sécurité** pour la détection et la prévention. Les principaux risques résidus concernent :

✅ **Excellents:** Secrets detection, SAST, dependency scanning  
🟡 **À améliorer:** Image signing, artifact integrity, log immutability  
🔴 **Critiques:** Vault/secret rotation, permission escalation controls  

**Recommandation:** Implémenter les mesures CRITICAL dans les 30 prochains jours.

---

**Version:** 2.0 | **Date:** 14 Avril 2026 | **Approuvé par:** DevSecOps Team
