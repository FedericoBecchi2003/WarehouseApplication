# 🚀 Quick Start Guide

Guida veloce per iniziare con la Warehouse Application in 5 minuti.

## 1️⃣ Prerequisites Check

```bash
# Verifica che Docker e Docker Compose siano installati
docker --version
docker-compose --version
```

## 2️⃣ Configurazione Iniziale

### Opzione A: Interattiva (Consigliato)

```bash
# Esegui lo script interattivo
./deploy.sh

# Scegli: 9) Full setup
```

### Opzione B: Manuale

```bash
# Copia il template di ambiente
cp .env.example .env

# Modifica .env con i tuoi credentials
nano .env
# O usa il tuo editor preferito
```

**Variabili obbligatorie:**
```bash
DOCKERHUB_USERNAME=your_username
DOCKERHUB_TOKEN=your_token
GITHUB_TOKEN=your_github_pat
```

## 3️⃣ Build Locale

```bash
# Build tutti i servizi
docker-compose build

# Oppure usa lo script
./deploy.sh build
```

## 4️⃣ Avvia i Servizi

```bash
# Avvia tutto
docker-compose up -d

# Verifica lo stato
docker-compose ps

# Oppure usa lo script
./deploy.sh start
```

## 5️⃣ Verifica Funzionamento

```bash
# Health check
./deploy.sh health

# O controlla manualmente
curl http://localhost:5001/health  # Order Service
curl http://localhost:5003/health  # Payment Service
curl http://localhost:5002/health  # Warehouse Service
```

## 📊 Visualizza i Log

```bash
# Tutti i log
docker-compose logs -f

# Log di un servizio specifico
docker-compose logs -f order-api
docker-compose logs -f payment-api
docker-compose logs -f warehouse-api

# Oppure usa lo script
./deploy.sh logs [service_name]
```

## 🛑 Ferma i Servizi

```bash
# Ferma tutto
docker-compose down

# Oppure
./deploy.sh stop
```

## 📋 Comandi Utili

```bash
# Lista dei container
docker-compose ps

# Riavvia un servizio
docker-compose restart order-api

# Ricostruisci un servizio
docker-compose build order-api

# Esci dal container in esecuzione
docker exec -it order-postgres psql -U ecommerce_user -d order_db

# Visualizza le immagini
docker images | grep warehouse

# Rimuovi le immagini locali
docker rmi warehouse-order-service:local
docker rmi warehouse-payment-service:local
docker rmi warehouse-warehouse-service:local

# Pulisci tutto (attenzione!)
./deploy.sh clean
```

## 🐳 Usa Immagini da DockerHub

Se le immagini sono già su DockerHub:

```bash
# Modifica .env con i tuoi dati
DOCKERHUB_USERNAME=your_username
ORDER_SERVICE_TAG=latest
PAYMENT_SERVICE_TAG=latest
WAREHOUSE_SERVICE_TAG=latest

# Pull e avvia
docker-compose pull
docker-compose up -d
```

## 🚀 Push su DockerHub

```bash
# Build locali
docker-compose build

# Push
./deploy.sh push

# O manualmente
docker tag warehouse-order-service:local $DOCKERHUB_USERNAME/warehouse-order-service:latest
docker push $DOCKERHUB_USERNAME/warehouse-order-service:latest
# Ripeti per gli altri servizi
```

## 🔗 Accedi ai Servizi

| Servizio | URL | Descrizione |
|----------|-----|-------------|
| Order API | http://localhost:5001 | Service Order |
| Warehouse API | http://localhost:5002 | Service Warehouse |
| Payment API | http://localhost:5003 | Service Payment |
| Order DB | localhost:5431 | PostgreSQL Order |
| Warehouse DB | localhost:5432 | PostgreSQL Warehouse |
| Payment DB | localhost:5433 | PostgreSQL Payment |
| Kafka | localhost:9092 | Message Broker |

## 📚 Documentazione Completa

- [DEPLOYMENT.md](DEPLOYMENT.md) - Guida completa di deployment
- [ARCHITECTURE.md](ARCHITECTURE.md) - Schema architetturale
- [.github/workflows/deploy.yml](.github/workflows/deploy.yml) - CI/CD Pipeline

## 🆘 Troubleshooting Rapido

### I servizi non partono

```bash
# Controlla i log
docker-compose logs

# Verifica le porte
lsof -i :5001
lsof -i :5002
lsof -i :5003

# Riavvia
docker-compose down
docker-compose up -d
```

### Errore di connessione NuGet

```bash
# Verifica il GITHUB_TOKEN
echo $GITHUB_TOKEN

# Ricrea le immagini
docker-compose build --no-cache
```

### Database non si connette

```bash
# Verifica che PostgreSQL sia in esecuzione
docker-compose ps | grep postgres

# Visualizza i log
docker-compose logs order-postgres
```

### Porta già in uso

```bash
# Uccidi il processo sulla porta
lsof -i :5001  # Vedi il PID
kill -9 <PID>

# O cambia la porta in docker-compose.yml
# Cambia "5001:8080" in "5011:8080"
```

## ✅ Checklist Post-Deployment

- [ ] Tutti i container sono in esecuzione (`docker-compose ps`)
- [ ] Health check passato (`./deploy.sh health`)
- [ ] Database connessi correttamente
- [ ] Kafka funzionante
- [ ] Microservizi comunicano via API
- [ ] Log puliti (no errori gravi)

## 🎯 Prossimi Passi

1. **Leggi la documentazione completa:**
   ```bash
   cat DEPLOYMENT.md
   cat ARCHITECTURE.md
   ```

2. **Configura GitHub Secrets:** Vai a repo → Settings → Secrets

3. **Fai un test commit:**
   ```bash
   git add .
   git commit -m "Initial deployment setup"
   git push origin main
   # Controlla GitHub Actions
   ```

4. **Testa i microservizi:**
   - Accedi agli endpoint API
   - Crea ordini, pagamenti
   - Verifica i log per i messaggi Kafka

## 📞 Support

Se hai problemi:

1. Controlla i log: `docker-compose logs -f`
2. Verifica i prerequisiti: `./deploy.sh check`
3. Leggi DEPLOYMENT.md sezione Troubleshooting
4. Consulta la documentazione di Docker/Docker Compose

---

**Ultima modifica:** Giugno 2026  
**Versione:** 1.0
