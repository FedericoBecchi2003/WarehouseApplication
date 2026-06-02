#!/bin/bash

# ========================================
# Warehouse Application - Common Commands
# ========================================
# Copia questi comandi e incollali nel terminale

# ========================================
# 📋 SETUP INIZIALE
# ========================================

# Copia il template ambiente
cp .env.example .env

# Modifica i tuoi credentials
nano .env

# Rendi lo script eseguibile
chmod +x deploy.sh

# ========================================
# 🏗️ BUILD
# ========================================

# Build tutti i servizi (locale)
docker-compose build

# Build solo Order Service
docker-compose build order-api

# Build solo Payment Service
docker-compose build payment-api

# Build solo Warehouse Service
docker-compose build warehouse-api

# Build senza cache (pulito)
docker-compose build --no-cache

# Usa lo script helper
./deploy.sh build

# ========================================
# 🚀 AVVIA SERVIZI
# ========================================

# Avvia tutto in background
docker-compose up -d

# Avvia e visualizza i log
docker-compose up

# Avvia solo i database
docker-compose up -d order-db payment-db warehouse-db kafka

# Avvia un servizio specifico
docker-compose up -d order-api

# Usa lo script helper
./deploy.sh start

# ========================================
# 🛑 FERMA SERVIZI
# ========================================

# Ferma tutto
docker-compose down

# Ferma e rimuovi volumi (attenzione!)
docker-compose down -v

# Ferma un servizio specifico
docker-compose stop order-api

# Riavvia un servizio
docker-compose restart order-api

# Usa lo script helper
./deploy.sh stop

# ========================================
# 📊 VISUALIZZA STATUS
# ========================================

# Status di tutti i container
docker-compose ps

# Status dei container con dettagli
docker-compose ps --format "table {{.Service}}\t{{.Status}}\t{{.Ports}}"

# Lista immagini Docker
docker images | grep warehouse

# Lista volumi
docker volume ls | grep warehouse

# ========================================
# 📜 LOG
# ========================================

# Log di tutti i servizi
docker-compose logs

# Log di tutti i servizi (follow)
docker-compose logs -f

# Log di un servizio specifico
docker-compose logs order-api
docker-compose logs payment-api
docker-compose logs warehouse-api

# Log di un servizio (follow)
docker-compose logs -f order-api

# Ultimi 50 righe di log
docker-compose logs --tail=50

# Log dal tempo di avvio
docker-compose logs order-api --since 5m

# Usa lo script helper
./deploy.sh logs
./deploy.sh logs order-api

# ========================================
# 🏥 HEALTH CHECK
# ========================================

# Health check interattivo
./deploy.sh health

# Health check manuale - Order Service
curl http://localhost:5001/health

# Health check manuale - Payment Service
curl http://localhost:5003/health

# Health check manuale - Warehouse Service
curl http://localhost:5002/health

# ========================================
# 🐘 DATABASE
# ========================================

# Accedi al database Order
docker exec -it order-postgres psql -U ecommerce_user -d order_db

# Accedi al database Payment
docker exec -it payment-postgres psql -U ecommerce_user -d payment_db

# Accedi al database Warehouse
docker exec -it warehouse-postgres psql -U ecommerce_user -d warehouse_db

# Query SQL da shell
docker exec -it order-postgres psql -U ecommerce_user -d order_db -c "SELECT version();"

# Dump database
docker exec order-postgres pg_dump -U ecommerce_user order_db > backup.sql

# ========================================
# 📨 KAFKA
# ========================================

# Accedi al container Kafka
docker exec -it ecommerce-kafka bash

# Lista topic Kafka
docker exec -it ecommerce-kafka kafka-topics --bootstrap-server localhost:29092 --list

# Crea topic
docker exec -it ecommerce-kafka kafka-topics --bootstrap-server localhost:29092 --create --topic orders --partitions 1 --replication-factor 1

# Leggi messaggi da topic
docker exec -it ecommerce-kafka kafka-console-consumer --bootstrap-server localhost:29092 --topic orders --from-beginning

# ========================================
# 🔍 TROUBLESHOOTING
# ========================================

# Verifica porta in uso
lsof -i :5001
lsof -i :5002
lsof -i :5003
lsof -i :5431
lsof -i :5432
lsof -i :5433
lsof -i :9092

# Uccidi processo sulla porta
kill -9 <PID>

# Verifica file docker-compose
docker-compose config

# Valida file docker-compose
docker-compose config --quiet && echo "Valid" || echo "Invalid"

# Controlla dipendenze
docker-compose config --resolve-image-digests

# ========================================
# 📦 DOCKER HUB
# ========================================

# Login a DockerHub
docker login

# Tag per DockerHub
docker tag warehouse-order-service:local your_user/warehouse-order-service:latest

# Push a DockerHub
docker push your_user/warehouse-order-service:latest

# Pull da DockerHub
docker pull your_user/warehouse-order-service:latest

# Usa lo script helper
./deploy.sh push

# ========================================
# 🔧 UTILITY
# ========================================

# Controlla utilizzo risorse
docker stats

# Ispeziona container
docker inspect order-api

# Visualizza variabili di ambiente
docker exec order-api env

# Copia file da container
docker cp order-api:/app/config.json ./config.json

# Esegui comando in container
docker exec -it order-api dotnet --version

# Rebuild without cache
docker-compose down
docker system prune -a
docker-compose build --no-cache

# ========================================
# 🧹 PULIZIA
# ========================================

# Rimuovi container fermati
docker container prune

# Rimuovi immagini non usate
docker image prune

# Rimuovi volumi non usati
docker volume prune

# Rimuovi tutto (attenzione!)
docker system prune -a --volumes

# Usa lo script helper
./deploy.sh clean

# ========================================
# 🚀 DEPLOY WORKFLOW
# ========================================

# 1. Sviluppa e testa
# ... modifica codice ...
./deploy.sh build
docker-compose up -d
./deploy.sh health

# 2. Commit e push
git add .
git commit -m "Feature: ..."
git push origin develop

# 3. GitHub Actions esegue automaticamente

# 4. Verifica su DockerHub
docker search warehouse-order-service

# 5. Deploy in produzione
export $(cat .env | grep -v '#' | xargs)
docker-compose pull
docker-compose up -d

# ========================================
# 📚 COMANDI UTILI
# ========================================

# Visualizza versione Docker
docker --version
docker-compose --version

# Visualizza configurazione
docker info

# Visualizza version del runtime
docker run --rm node:18 node --version

# Background process status
jobs

# Porta processo in foreground
fg %1

# ========================================
# 💡 TIPS & TRICKS
# ========================================

# Abbreviazione comandi
alias dc="docker-compose"
alias dp="docker ps"
alias dps="docker ps -a"
alias di="docker images"

# Uso delle abbreviazioni
dc ps
dc logs -f
dp
dps

# Export log in file
docker-compose logs > app.log

# Monitoraggio real-time
watch -n 1 'docker-compose ps'

# ========================================
# 📞 HELPFUL ALIASES
# ========================================

# Aggiungi a ~/.bashrc o ~/.zshrc

# Docker Compose
alias dc='docker-compose'
alias dcup='docker-compose up -d'
alias dcdown='docker-compose down'
alias dcps='docker-compose ps'
alias dclogs='docker-compose logs -f'
alias dcbuild='docker-compose build'

# Docker
alias dp='docker ps'
alias dpa='docker ps -a'
alias di='docker images'
alias dip='docker images prune'

# Warehouse Application
alias wh-health='./deploy.sh health'
alias wh-build='./deploy.sh build'
alias wh-start='./deploy.sh start'
alias wh-stop='./deploy.sh stop'
alias wh-logs='./deploy.sh logs'
alias wh-push='./deploy.sh push'

# ========================================
# 🎯 QUICK WORKFLOWS
# ========================================

# WORKFLOW 1: Development Loop
# =====================
git pull
./deploy.sh build
docker-compose up -d
./deploy.sh health
docker-compose logs -f

# WORKFLOW 2: Deploy to Production
# ================
docker-compose pull
docker-compose down -v
docker-compose up -d
./deploy.sh health

# WORKFLOW 3: Debug Specific Service
# ================
docker-compose logs -f order-api
docker exec -it order-api bash

# WORKFLOW 4: Full Cleanup & Restart
# ================
./deploy.sh stop
docker system prune -a --volumes
./deploy.sh build
docker-compose up -d
./deploy.sh health

# ========================================
# 📖 DOCUMENTATION
# ========================================

# Leggi guide
cat QUICK_START.md
cat DEPLOYMENT.md
cat ARCHITECTURE.md
cat SETUP_SUMMARY.md

# ========================================
