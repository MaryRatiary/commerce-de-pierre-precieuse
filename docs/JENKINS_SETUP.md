# Jenkins Setup Guide - CI/CD Pipeline

## 📋 Table des Matières

1. [Installation Jenkins](#installation-jenkins)
2. [Configuration Initiale](#configuration-initiale)
3. [Intégration GitHub](#intégration-github)
4. [Setup Pipelines](#setup-pipelines)
5. [Plugins Requis](#plugins-requis)
6. [Sécurité Jenkins](#sécurité-jenkins)
7. [Troubleshooting](#troubleshooting)

---

## 🚀 Installation Jenkins

### Option 1: Docker (Recommandé)

```bash
# Créer réseau Docker
docker network create jenkins

# Volume pour persister data
docker volume create jenkins_home

# Démarrer Jenkins
docker run -d \
  --name jenkins \
  --restart unless-stopped \
  -u jenkins:jenkins \
  -p 8080:8080 \
  -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  jenkins/jenkins:lts

# Récupérer le token initial
docker logs jenkins 2>&1 | grep -A 5 "initialAdminPassword"
```

### Option 2: Installation Locale (macOS)

```bash
# Avec Homebrew
brew install jenkins-lts

# Démarrer service
brew services start jenkins-lts

# Accès
# http://localhost:8080
```

### Option 3: Installation Linux

```bash
# Ajouter repository
sudo wget -q -O - https://pkg.jenkins.io/debian-stable/jenkins.io.key | sudo apt-key add -
sudo sh -c 'echo deb https://pkg.jenkins.io/debian-stable binary/ > /etc/apt/sources.list.d/jenkins.list'

# Installation
sudo apt-get update
sudo apt-get install jenkins

# Démarrer service
sudo systemctl start jenkins
sudo systemctl enable jenkins
```

---

## ⚙️ Configuration Initiale

### 1. Setup Assistant

**Step 1: Unlock Jenkins**
```bash
# http://localhost:8080
# Copier le token d'initialAdminPassword
cat /var/lib/jenkins/secrets/initialAdminPassword
```

**Step 2: Install Plugins**
- ✅ Sélectionner "Install suggested plugins"
- ✅ Attendre installation (~10 min)

**Step 3: Create Admin User**
```
Username: admin
Password: [Secure password]
Full name: Jenkins Administrator
Email: admin@company.com
```

**Step 4: Configure Jenkins URL**
```
Jenkins URL: http://jenkins.company.com:8080/
(ou http://localhost:8080 pour local)
```

### 2. Configuration Système

**Manage Jenkins → System Configuration**

#### Global Properties

```
Environment variables:
JENKINS_NODE_COOKIE=dontKillMe
DOCKER_HOST=unix:///var/run/docker.sock
```

#### Location

```
Jenkins URL: http://localhost:8080/
System Admin e-mail: admin@company.com
```

#### Email Notification

```
SMTP Server: smtp.gmail.com
SMTP Port: 587
Default sender: jenkins@company.com
```

---

## 🔗 Intégration GitHub

### 1. Créer GitHub Token

```bash
# Sur GitHub:
# Settings → Developer settings → Personal access tokens → Tokens (classic)
# Permissions requis:
- repo (full control)
- admin:repo_hook (hook access)
- user:email (email access)
```

### 2. Ajouter Credentials Jenkins

**Manage Jenkins → Credentials → System → Global credentials**

```
1. Kind: Username with password
   Username: [Your GitHub username]
   Password: [Personal access token]
   ID: github-credentials
   Description: GitHub API Token

2. Kind: SSH Username with private key
   Username: git
   Private Key: [Your SSH key]
   ID: github-ssh
   Description: GitHub SSH key
```

### 3. Configuration GitHub Plugin

**Manage Jenkins → System Configuration → GitHub**

```
GitHub Server:
- Name: GitHub
- API URL: https://api.github.com
- Credentials: github-credentials
- Test connection
```

### 4. Webhook GitHub

**Repository Settings → Webhooks**

```
Payload URL: http://jenkins.company.com:8080/github-webhook/
Content type: application/json
Events: Push events + Pull request events
✅ Active
```

---

## 🔄 Setup Pipelines

### Pipeline 1: Main CI/CD

**New Item**
```
Name: ecom-ci-cd-main
Type: Pipeline
```

**Pipeline Configuration**

```
Definition: Pipeline script from SCM
SCM: Git
Repository URL: https://github.com/YOUR-ORG/e-com.git
Credentials: github-credentials
Branch: main
Script Path: .jenkins/Jenkinsfile
```

**Build Triggers**

```
☑ GitHub hook trigger for GITScm polling
Trigger only if changes in subdirectories:
  - backend/
  - frontend/
  - .jenkins/
  - .github/
```

**Advanced Options**

```
Lightweight checkout: ☑
Fast remote polling: ☑
```

### Pipeline 2: Backend Only

```
Name: ecom-backend
Type: Pipeline
Script Path: .jenkins/Jenkinsfile.backend
```

### Pipeline 3: Frontend Only

```
Name: ecom-frontend
Type: Pipeline
Script Path: .jenkins/Jenkinsfile.frontend
```

---

## 📦 Plugins Requis

### Installation Automatique

**Manage Jenkins → Plugins → Available plugins**

```bash
# Search et install:
- Pipeline
- GitHub Integration
- Docker Pipeline
- Blue Ocean
- Email Extension
- Slack Notification
- SonarQube Scanner
- OWASP Dependency-Check
- Performance
- JUnit
- Cobertura
```

### Ou via Script

```groovy
// Manage Jenkins → Script Console
def pluginManager = Jenkins.instance.pluginManager
def updateCenter = Jenkins.instance.getUpdateCenter()

["pipeline-model-definition", "github-branch-source", "docker-pipeline", 
 "blueocean", "email-ext", "slack"].each { pluginName ->
    if (!pluginManager.plugins.any { it.getShortName() == pluginName }) {
        println("Installing ${pluginName}...")
        def plugin = updateCenter.getPlugin(pluginName)
        plugin.deploy()
    }
}
```

---

## 🔒 Sécurité Jenkins

### 1. Authentication & Authorization

**Manage Jenkins → Security**

```
Security Realm: Jenkins' own user database
Authorization: Role-Based Strategy
```

**User Roles**

```
Admin role:
- administer (all)

Developer role:
- job-read (all)
- job-build (all)
- job-cancel (all)
- job-discover (all)

Viewer role:
- job-read (all)
- job-discover (all)
```

### 2. Credentials Security

**Manage Jenkins → Security → Credentials**

```
- Store safely (encrypted)
- Use JobDSL/Pipeline for references
- Rotate credentials regularly
- Audit access logs
```

### 3. Pipeline Security

```groovy
// Dans .jenkins/Jenkinsfile

// ✅ Use credentials safely
withCredentials([
    string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN'),
    usernamePassword(credentialsId: 'github-credentials', 
                     usernameVariable: 'GIT_USER', 
                     passwordVariable: 'GIT_PASS')
]) {
    // Ne JAMAIS afficher dans les logs
    sh 'echo $SONAR_TOKEN'  // ❌ UNSAFE
    sh 'sonar-scanner -Dsonar.login=${SONAR_TOKEN}'  // ✅ SAFE
}
```

### 4. CSRF Protection

**Manage Jenkins → Security**

```
✅ Enable script security for Job DSL
✅ Enable script security for Groovy
✅ Prevent Cross Site Request Forgery exploits
```

---

## 🧪 Test & Validation

### 1. Test Connection GitHub

```bash
# Manage Jenkins → System Configuration → GitHub
# Cliquer "Test connection"
# Doit afficher: "Credentials verified for user: [username]"
```

### 2. Test Pipeline

**Déclencher build manuel**

```bash
# Jenkins UI → ecom-ci-cd-main → Build Now
# Vérifier:
- ✅ Repo cloné correctement
- ✅ Tous les stages exécutés
- ✅ Logs disponibles
- ✅ Artifacts archivés
```

### 3. Test GitHub Integration

```bash
# Push commit to develop branch
git add .
git commit -m "test: jenkins integration"
git push origin develop

# Vérifier:
- ✅ Webhook déclenche build automatiquement
- ✅ GitHub status check updated
- ✅ PR shows build status
```

---

## 📊 Monitoring & Maintenance

### 1. Logs

```bash
# Voir tous les logs
# Manage Jenkins → System Log → All logs

# Ou via container
docker logs -f jenkins
```

### 2. Performance Tuning

```groovy
// Manage Jenkins → Configure System

Java Heap Size: -Xmx2g -Xms512m
Number of executors: 4
Process queue: enabled
```

### 3. Backup

```bash
# Backup Jenkins data
docker exec jenkins tar czf - /var/jenkins_home > backup.tar.gz

# Restore
tar xzf backup.tar.gz -C /path/to/restore
```

---

## 🐛 Troubleshooting

### Build Failed: Can't clone repository

```bash
# Solution:
1. Vérifier credentials GitHub
2. Tester SSH access:
   ssh -T git@github.com
3. Vérifier branch exists
4. Check webhook
```

### Pipeline Timeout

```bash
# Solution:
1. Augmenter timeout dans Jenkinsfile:
   options {
       timeout(time: 2, unit: 'HOURS')
   }
2. Optimiser build (cache, parallel)
3. Augmenter resources Jenkins (RAM, CPU)
```

### Plugin Not Loaded

```bash
# Solution:
1. Manage Jenkins → Plugin Manager
2. Search plugin name
3. Install sans restart
4. Ou restart Jenkins: systemctl restart jenkins
```

### Out of Memory

```bash
# Solution:
# Augmenter heap size

# Docker:
docker update --memory 4g jenkins

# Systemd:
sudo nano /etc/default/jenkins
JAVA_ARGS="-Xmx4g -Xms512m"
```

---

## ✅ Checklist Déploiement

- [ ] Jenkins installé et accessible
- [ ] Admin user créé
- [ ] Plugins installés
- [ ] GitHub intégré avec webhook
- [ ] Credentials configurés
- [ ] Pipelines créés
- [ ] Test build manuel réussi
- [ ] GitHub webhook déclenche build
- [ ] Logs consultables
- [ ] Backup configuré
- [ ] Monitoring en place
- [ ] Équipe formée

---

## 📞 Support

Pour questions/issues:
- Jenkins Docs: https://www.jenkins.io/doc/
- GitHub Integration: https://plugins.jenkins.io/github/
- Slack: #devops-jenkins

