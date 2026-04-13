# ⚡ QUICK START - 5 Minutes

## 1️⃣ Sur GitHub (5 min)

### A. Crée un repository public
```
https://github.com/new
- Name: e-com
- Public (pour GitHub Actions gratuit)
- Create repository
```

### B. Push ton code local
```bash
cd /Users/RatiaryMario/e-com

git remote add origin https://github.com/TON-USERNAME/e-com.git
git branch -M main
git push -u origin main
```

## 2️⃣ Configure GitHub Secrets (2 min)

Va dans : **Settings → Secrets and variables → Actions → New repository secret**

Ajoute ces secrets (minimum nécessaire):
- `SONARQUBE_TOKEN` = "demo" (ou ton token SonarQube)
- `SLACK_WEBHOOK` = "https://hooks.slack.com/..." (optionnel, pour notifications)

⚠️ **C'est tout ce qu'il faut !** Les workflows GitHub Actions vont se déclencher automatiquement.

---

## 3️⃣ Vérifie que ça marche

Va dans : **Actions tab**

Tu dois voir :
- ✅ Backend CI/CD Pipeline (running...)
- ✅ Frontend CI/CD Pipeline (running...)

Attends 10-15 min. Clique sur les runs pour voir les logs.

---

## 4️⃣ Résultats attendus

✅ Les workflows font automatiquement:
- 🔐 Scan secrets (Gitleaks)
- 🛡️ Analyse code (Semgrep)
- 📦 Check dépendances (npm audit, dotnet)
- 🔨 Build backend (.NET)
- 🔨 Build frontend (React)
- 🐳 Scan images Docker (Trivy)
- 📊 Résultats visibles dans GitHub Security tab

---

## 5️⃣ JENKINS (Optionnel - Skip si pas besoin)

Si tu veux Jenkins pour automatiser aussi:

```bash
# Sur ta machine (macOS)
brew install jenkins

# Démarre Jenkins
jenkins

# Accède à http://localhost:8080
# Username: admin
# Password: (regarder terminal pour initial password)
```

Puis: **New Item → Pipeline → Configure**
```
Pipeline script from SCM
- Git
- Repository URL: https://github.com/TON-USERNAME/e-com.git
- Script path: .jenkins/Jenkinsfile
```

Save → Build Now

Mais **honnêtement: GitHub Actions suffit, Jenkins pas nécessaire.**

---

## 🎯 Workflow Quotidien (Pour Toi)

```bash
# 1. Fais des changements
vim backend/Program.cs

# 2. Commit et push
git add .
git commit -m "feat: add new endpoint"
git push

# 3. FINI ! ✅
# GitHub Actions lance automatiquement:
# - Tests
# - Scans sécurité
# - Build
# - Rapports

# 4. Vérifie les résultats
# Actions tab → Voir le dernier run
```

**Pas besoin de faire quoi que ce soit d'autre !**

---

## ⚠️ Si un workflow échoue

Clique sur le run échoué → Scrolle logs → Voir le problème

Exemples courants:
- ❌ "npm: command not found" → Frontend Dockerfile manque Node
- ❌ "dotnet: command not found" → Backend Dockerfile manque .NET SDK
- ❌ "Secret not found" → Ajoute le secret manquant dans Settings

---

## 📊 Voir les Résultats de Sécurité

**Security tab** → Tu verras:
- Dependabot alerts
- Code scanning (Semgrep)
- Secret scanning (Gitleaks)
- Vulnerability alerts

---

## ✅ Checklist Finale

- [ ] Repository créé sur GitHub
- [ ] Code pusché (`git push`)
- [ ] Secrets configurés (Settings → Secrets)
- [ ] Actions tab → workflows running
- [ ] Attendre 15 min pour résultats
- [ ] Regarder les logs → OK ✅

**C'est TOUT. Vraiment. Tu peux arrêter là.**

