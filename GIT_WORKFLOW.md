# 🔀 Git Workflow Guide

Questa guida ti mostra come lavorare con Git per il progetto Warehouse Application.

## 📋 Indice

1. [Setup Iniziale](#setup-iniziale)
2. [Daily Workflow](#daily-workflow)
3. [Branch Strategy](#branch-strategy)
4. [CI/CD Integration](#cicd-integration)
5. [Commit Message Convention](#commit-message-convention)
6. [Comandi Utili](#comandi-utili)
7. [Troubleshooting Git](#troubleshooting-git)

---

## 🔧 Setup Iniziale

### Clone Repository

```bash
# HTTPS
git clone https://github.com/your-username/WarehouseApplication.git
cd WarehouseApplication

# SSH (se hai chiave configurata)
git clone git@github.com:your-username/WarehouseApplication.git
```

### Configura Git Localmente

```bash
# Configura utente
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# Per solo questo repository
git config user.name "Your Name"
git config user.email "your.email@example.com"

# Verifica configurazione
git config --list
```

### Genera SSH Key (Optional)

```bash
# Genera chiave SSH
ssh-keygen -t ed25519 -C "your.email@example.com"

# Aggiungi a ssh-agent
eval "$(ssh-agent -s)"
ssh-add ~/.ssh/id_ed25519

# Copia la chiave pubblica su GitHub
cat ~/.ssh/id_ed25519.pub  # Copia questo content
# GitHub → Settings → SSH and GPG keys → New SSH key
```

---

## 📅 Daily Workflow

### 1️⃣ Inizio della Giornata - Aggiorna il Local

```bash
# Posizionati su main
git checkout main

# Scarica gli ultimi cambiamenti
git pull origin main

# Verifica status
git status
```

### 2️⃣ Crea un Branch per il Tuo Lavoro

```bash
# Crea branch da main
git checkout -b feature/my-feature-name

# O: crea da develop (se usi strategy più complessa)
git checkout -b feature/my-feature-name origin/develop

# Verifica branch corrente
git branch -a
```

### 3️⃣ Sviluppa e Commit

```bash
# Vedi cosa è cambiato
git status

# Aggiungi file specifici
git add OrderService/
git add PaymentService/

# Oppure aggiungi tutto
git add .

# Verifica i cambiamenti prima di committare
git diff --cached

# Committa con messaggio significativo
git commit -m "feat: Add payment validation logic"
```

### 4️⃣ Mantieni il Branch Aggiornato

```bash
# Mentre lavori, main potrebbe aver ricevuto updatesDI
# Aggiorna il tuo branch:

# Metodo 1: Rebase (preferred per commits lineari)
git fetch origin
git rebase origin/main

# Metodo 2: Merge (crea commit di merge)
git fetch origin
git merge origin/main
```

### 5️⃣ Push e Apri Pull Request

```bash
# Pushare il branch
git push origin feature/my-feature-name

# GitHub ti mostrerà un link per creare un PR

# Online (su GitHub):
# 1. Clicca "Compare & pull request"
# 2. Descrivi i cambiamenti
# 3. Seleziona i reviewers
# 4. Clicca "Create pull request"
```

### 6️⃣ Dopo la Review e Merge

```bash
# Dopo che il PR è stato merged:

# Aggiorna main locale
git fetch origin
git checkout main
git pull origin main

# Cancella il branch locale (opzionale ma pulito)
git branch -d feature/my-feature-name

# Cancella dal remote se necessario
git push origin --delete feature/my-feature-name
```

---

## 🌳 Branch Strategy

### Git Flow Strategy

Raccomandiamo di usare **Git Flow** per questo progetto:

```
main         (production, versioned)
  ↑
  └─ develop (pre-production)
      ↑
      ├─ feature/xxx (nuove feature)
      ├─ bugfix/xxx (correzioni di bug)
      ├─ hotfix/xxx (emergency fixes per production)
      └─ release/xxx (release candidates)
```

### Creazione Branch

```bash
# Feature
git checkout -b feature/order-validation develop

# Bugfix
git checkout -b bugfix/payment-timeout develop

# Hotfix (SOLO per production issues)
git checkout -b hotfix/security-patch main

# Release (per preparation di nuova versione)
git checkout -b release/1.0.0 develop
```

### Nomi Branch Significativi

```
✓ BUONI:
  feature/add-payment-validation
  bugfix/fix-order-timeout
  hotfix/security-vuln
  feature/order-service/add-logging

✗ CATTIVI:
  feature/fix1
  my-changes
  new_branch
  temporary
```

---

## 🔄 CI/CD Integration

### Trigger GitHub Actions

GitHub Actions automaticamente:

1. **Al push su main/develop:**
   - Build Docker images
   - Push a DockerHub
   - Tag images

2. **Al pull request:**
   - Run linting (future)
   - Run tests (future)
   - Build check

### Monitorare CI/CD

```bash
# Online
# GitHub → Actions → Workflow Name → Latest Run

# Comandi git per verificare
git log -1 --stat
git show HEAD
```

---

## 💬 Commit Message Convention

Usa **Conventional Commits** per messaggi chiari:

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Tipi Validi

```
feat:       Nuova feature
fix:        Bug fix
docs:       Documentazione
style:      Formatting (no code changes)
refactor:   Code refactoring
perf:       Performance improvement
test:       Aggiunta/modifica test
chore:      Build, deps, tooling
ci:         CI/CD configuration
```

### Esempi

```bash
# Feature
git commit -m "feat(order-service): Add order validation"

# Fix
git commit -m "fix(payment-api): Resolve timeout issue"

# Documentation
git commit -m "docs: Update deployment guide"

# Multiple lines
git commit -m "feat(warehouse-service): Add inventory tracking

- Implemented stock level updates
- Added low stock alerts
- Integrated with Kafka events"
```

### Raccomandazioni

- ✅ Inizia con lowercase
- ✅ Usa imperativo ("add", non "added")
- ✅ Non terminare con punto
- ✅ Max 50 caratteri per subject
- ✅ Usa body per dettagli (72 caratteri per linea)

---

## 💻 Comandi Utili

### Visualizzazione

```bash
# Log di commit
git log
git log --oneline
git log --oneline -10
git log --graph --oneline --all

# Differenze
git diff                    # Unstaged changes
git diff --cached           # Staged changes
git diff HEAD~1             # Vs last commit
git diff main develop       # Tra branch

# Status
git status
git status -s               # Short format
```

### Modifica

```bash
# Modifica ultimo commit (se non pushato)
git commit --amend
git commit --amend --no-edit  # Mantieni messaggio

# Unstage file
git reset HEAD file.txt

# Undo changes
git checkout -- file.txt
git restore file.txt        # Newer syntax

# Undo commit (mantieni cambiamenti)
git reset --soft HEAD~1

# Undo commit (scarta cambiamenti)
git reset --hard HEAD~1
```

### Stash (Salva Lavoro Temporaneo)

```bash
# Salva cambiamenti non committati
git stash

# Lista stash
git stash list

# Recupera stash
git stash pop               # Applica e rimuove
git stash apply             # Applica senza rimuovere

# Rimuovi stash
git stash drop
```

### Branch

```bash
# Lista branch
git branch                  # Local
git branch -a               # Tutti

# Cambia branch
git checkout develop
git switch develop          # Newer syntax

# Crea e cambia
git checkout -b feature/xxx
git switch -c feature/xxx   # Newer syntax

# Elimina branch
git branch -d feature/xxx
git branch -D feature/xxx   # Force
```

### Remote

```bash
# Lista remote
git remote -v

# Aggiungi remote
git remote add upstream https://github.com/original/repo

# Fetch
git fetch origin
git fetch --all

# Push
git push origin main
git push origin feature/xxx

# Pull
git pull origin main
git pull --rebase origin main  # Rebase vs merge
```

### Tag (Versioning)

```bash
# Lista tag
git tag
git tag -l "v1.*"

# Crea tag
git tag v1.0.0
git tag -a v1.0.0 -m "Release version 1.0.0"

# Push tag
git push origin v1.0.0
git push origin --tags      # Tutti i tag
```

---

## 🆘 Troubleshooting Git

### Problem: "Permission denied (publickey)"

```bash
# Soluzione 1: Usa HTTPS
git clone https://github.com/user/repo.git

# Soluzione 2: Configura SSH
ssh-keygen -t ed25519 -C "your@email.com"
# Aggiungi la chiave a GitHub Settings

# Soluzione 3: Usa credential manager
git config --global credential.helper osxkeychain
```

### Problem: "Failed to push some refs"

```bash
# Significa che remote ha cambiamenti non locali
# Soluzione:

git fetch origin
git pull origin main        # Merge approach
# Oppure
git pull --rebase origin main  # Rebase approach
git push origin main
```

### Problem: "Detached HEAD"

```bash
# Accade quando checkout un commit specifico
# Soluzione:

git log --oneline           # Vedi dove sei
git checkout main           # Torna a main
git checkout -b new-branch  # O crea nuovo branch da qui
```

### Problem: "Your branch is ahead of origin"

```bash
# Hai local commits non pushati
# Soluzione:

git push origin <branch-name>
```

### Problem: Ho committato sulla branch sbagliata

```bash
# Crea la branch giusta
git branch feature/correct-name

# Resetta la branch sbagliata
git reset --hard origin/main

# Cambia alla branch giusta
git checkout feature/correct-name
```

### Problem: Ho committato il file sbagliato

```bash
# Opzione 1: Rimuovi file dal commit precedente
git reset --soft HEAD~1
git reset HEAD file-to-remove.txt
git commit -m "Commit message"

# Opzione 2: Aggiungi solo file corretti
git reset HEAD~1
git add file1.txt file2.txt
git commit -m "Correct commit"
```

### Problem: Ho fatto merge sbagliato

```bash
# Revert il merge commit
git revert -m 1 <merge-commit-hash>

# Oppure reset
git reset --hard HEAD~1  # Attenzione! Scarta i cambiamenti
```

---

## 📊 Comandi Avanzati

### Interactive Rebase (Cleanup Commits)

```bash
# Rielabora ultimi 3 commit
git rebase -i HEAD~3

# Nel editor:
# - pick = usa commit
# - reword = modifica messaggio
# - squash = unisci con precedente
# - drop = rimuovi
```

### Cherry Pick (Copia Commit)

```bash
# Copia un commit da un'altra branch
git cherry-pick <commit-hash>

# Copia multipli
git cherry-pick <hash1> <hash2> <hash3>
```

### Grep (Cerca in Commit)

```bash
# Cerca nei messaggi di commit
git log --grep="payment"

# Cerca nel codice in commit
git log -p -S "specificSearch"
```

### Bisect (Trova Bug)

```bash
# Ricerca binaria per trovare il commit che ha introdotto il bug
git bisect start
git bisect bad HEAD
git bisect good v1.0
# Git ti permette di testare commit intermedi
# Indica se è buono o cattivo
git bisect good  # oppure git bisect bad
# Ripeti fino a trovarlo
```

---

## ✅ Pre-Push Checklist

Prima di fare push:

```bash
☐ git status                   # Tutto committato?
☐ git log --oneline -5         # Messaggi ok?
☐ git diff origin/main         # Cambiamenti ok?
☐ docker-compose build         # Builds?
☐ ./deploy.sh health           # Services ok?
☐ git push origin branch-name  # Push!
```

---

## 📚 Risorse Utili

- [Git Documentation](https://git-scm.com/doc)
- [GitHub Guides](https://guides.github.com/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git Cheat Sheet](https://github.github.com/training-kit/downloads/github-git-cheat-sheet.pdf)

---

## 🎯 Team Guidelines

**Per il tuo team:**

1. Usa sempre **descriptive commit messages**
2. Fai un **pull request** per code review
3. Non pushare direttamente su main
4. Usa i **branch names suggeritin**
5. Squash commits prima di merge (per main)
6. Delete branch dopo merge

---

**Riferimento Rapido:** Salva questo file nei tuoi bookmarks!

**Ultimo aggiornamento:** Giugno 2026
