# Warehouse Application - Microservizi Deployment Guide

Una guida completa per deployare i 3 microservizi (Order, Payment, Warehouse) su DockerHub con CI/CD automatico tramite GitHub Actions.

## 📋 Architettura

```
┌─────────────────────────────────────────────────────────┐
│                  Warehouse Application                   │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  ┌──────────────────┐  ┌──────────────────┐             │
│  │  Order Service   │  │ Payment Service  │  Warehouse  │
│  │                  │  │                  │  Service    │
│  │ • Business       │  │ • Business       │  • Business │
│  │ • Repository     │  │ • Repository     │  • Repo     │
│  │ • ClientHttp     │  │ • ClientHttp     │  • ClientHttp│
│  │ • WebApi         │  │ • WebApi         │  • WebApi   │
│  └──────────────────┘  └──────────────────┘  └──────────┘
│         ↓                    ↓                    ↓
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────┐
│  │  PostgreSQL      │  │  PostgreSQL      │  │PostgreSQL│
│  │  (Order DB)      │  │  (Payment DB)    │  │(Warehouse)
│  └──────────────────┘  └──────────────────┘  └──────────┘
│              ↓                    ↓                ↓
│              └────────────────────┴────────────────┘
│                          │
│                    ┌─────▼──────┐
│                    │   Kafka    │
│                    │  (Events)  │
│                    └────────────┘
└─────────────────────────────────────────────────────────┘
```

## 🚀 Prerequisiti

- **Docker** e **Docker Compose** (Docker Desktop su Mac)
- **GitHub Repository** con accesso
- **DockerHub Account** con credentials
- **GitHub Personal Access Token** (PAT) per NuGet packages
- **.NET 8 SDK** (per sviluppo locale)

## 📝 Configurazione Iniziale

### 1. Crea il file `.env` locale

Copia `.env.example` in `.env`:

```bash
cp .env.example .env
```

Modifica i valori:

```bash
# DockerHub credentials
DOCKERHUB_USERNAME=your_dockerhub_username
DOCKERHUB_TOKEN=your_dockerhub_token

# GitHub token (read:packages)
GITHUB_TOKEN=your_github_pat_token
```

### 2. Configura GitHub Secrets

Nel tuo repository, vai a **Settings → Secrets and variables → Actions** e aggiungi:

| Secret | Valore |
|--------|--------|
| `DOCKERHUB_USERNAME` | Il tuo username DockerHub |
| `DOCKERHUB_TOKEN` | Il tuo access token DockerHub |
| `GITHUB_TOKEN` | Il tuo GitHub PAT (read:packages) |

### 3. Crea i repository DockerHub

Nel tuo account DockerHub, crea tre repository:

- `warehouse-order-service`
- `warehouse-payment-service`
- `warehouse-warehouse-service`

## 🔄 CI/CD Pipeline (GitHub Actions)

### Workflow automatico: `.github/workflows/deploy.yml`

Triggerato quando effettui un push su `main` o `develop`:

```yaml
on:
  push:
    branches:
      - main
      - develop
    paths:
      - 'OrderService/**'
      - 'PaymentService/**'
      - 'WarehouseService/**'
      - 'docker-compose.yml'
```

### Cosa fa il workflow:

1. **Build per ogni microservizio in parallelo**
   - Order Service
   - Payment Service
   - Warehouse Service

2. **Per ogni build:**
   - Estrae i metadati (tag, version)
   - Effettua il login a DockerHub
   - Build l'immagine Docker multi-stage
   - Push su DockerHub (solo per push su main/develop)

3. **Tagging automatico:**
   - `latest` per il branch principale
   - `{branch-name}` per i branch
   - `{sha}` per il commit
   - `{version}` per i tag semantici

## 📦 Build Locale

### Build un singolo servizio:

```bash
# Order Service
docker build -f OrderService/Dockerfile \
  --build-arg GITHUB_TOKEN=$GITHUB_TOKEN \
  -t warehouse-order-service:local .

# Payment Service
docker build -f PaymentService/Dockerfile \
  --build-arg GITHUB_TOKEN=$GITHUB_TOKEN \
  -t warehouse-payment-service:local .

# Warehouse Service
docker build -f WarehouseService/Dockerfile \
  --build-arg GITHUB_TOKEN=$GITHUB_TOKEN \
  -t warehouse-warehouse-service:local .
```

### Build tutti i servizi:

```bash
docker-compose build
```

## 🐳 Deployment con Docker Compose

### Opzione 1: Build Locale

```bash
# Avvia tutti i servizi (build locale)
docker-compose up -d

# Visualizza i log
docker-compose logs -f

# Ferma i servizi
docker-compose down
```

### Opzione 2: Usa immagini da DockerHub

```bash
# Carica i valori dal file .env
export $(cat .env | grep -v '#' | xargs)

# Avvia con le immagini da DockerHub
docker-compose pull
docker-compose up -d
```

## 🧪 Verifica il Deployment

### 1. Controlla lo stato dei container:

```bash
docker-compose ps
```

Output atteso:
```
NAME                COMMAND                  SERVICE      STATUS      PORTS
order-service       "dotnet Order.WebAp…"   order-api    Up 2 mins   0.0.0.0:5001->8080/tcp
order-postgres      "docker-entrypoint.s…"  order-db     Up 2 mins   0.0.0.0:5431->5432/tcp
payment-service     "dotnet PaymentServi…"  payment-api  Up 2 mins   0.0.0.0:5003->8080/tcp
...
```

### 2. Test degli endpoint API:

```bash
# Order Service
curl http://localhost:5001/health

# Payment Service
curl http://localhost:5003/health

# Warehouse Service
curl http://localhost:5002/health
```

### 3. Visualizza i log:

```bash
# Log di un servizio specifico
docker-compose logs order-api -f
docker-compose logs payment-api -f
docker-compose logs warehouse-api -f

# Log di tutti i servizi
docker-compose logs -f
```

### 4. Accedi ai database:

```bash
# Order DB
docker exec -it order-postgres psql -U ecommerce_user -d order_db

# Payment DB
docker exec -it payment-postgres psql -U ecommerce_user -d payment_db

# Warehouse DB
docker exec -it warehouse-postgres psql -U ecommerce_user -d warehouse_db
```

### 5. Testa Kafka:

```bash
# Accedi al container Kafka
docker exec -it ecommerce-kafka bash

# Lista i topic
kafka-topics --bootstrap-server localhost:29092 --list

# Crea un topic di test
kafka-topics --bootstrap-server localhost:29092 --create --topic test-topic --partitions 1 --replication-factor 1
```

## 📊 Monitoraggio

### Visualizza le immagini Docker:

```bash
docker images | grep warehouse
```

### Controlla l'utilizzo delle risorse:

```bash
docker stats
```

### Visualizza i volumi:

```bash
docker volume ls | grep -E "order|payment|warehouse"
```

## 🔧 Troubleshooting

### I servizi non partono

```bash
# Controlla i log
docker-compose logs

# Verifica le porte non occupate
lsof -i :5001
lsof -i :5002
lsof -i :5003
lsof -i :5431
lsof -i :5432
lsof -i :5433
lsof -i :9092
```

### Errore di autenticazione NuGet

```bash
# Verifica che il GITHUB_TOKEN sia impostato
echo $GITHUB_TOKEN

# Ricrea le immagini
docker-compose build --no-cache
```

### Immagini non trovate su DockerHub

```bash
# Verifica il login
docker login

# Pull manuale
docker pull your_username/warehouse-order-service:latest
```

### Database non si connette

```bash
# Verifica che i database siano in esecuzione
docker-compose ps | grep postgres

# Controlla la variabile di connessione
docker-compose config | grep -A 5 ConnectionStrings
```

## 🔐 Sicurezza - Best Practices

### Per Produzione:

1. **Non commitcare `.env`** (è già in `.gitignore`)

2. **Usa secrets manager:**
   ```yaml
   # Usa GitHub Secrets, AWS Secrets Manager, o simili
   ```

3. **Abilita HTTPS:**
   ```bash
   # Configura in Program.cs
   app.UseHttpsRedirection();
   ```

4. **Cambia le password di default:**
   ```yaml
   POSTGRES_PASSWORD: $(generate-secure-password)
   ```

5. **Implementa rate limiting e CORS**

6. **Abilita logging e monitoring:**
   ```bash
   export LOG_LEVEL=Information
   ```

## 📚 Struttura Progetti

```
WarehouseApplication/
├── OrderService/
│   ├── OrderService.Business/       (Business logic)
│   ├── OrderService.ClientHttp/     (HTTP client)
│   ├── OrderService.Repository/     (Data access)
│   ├── OrderService.Shared/         (DTOs)
│   ├── OrderService.WebApi/         (API entry point)
│   └── Dockerfile                   (Multi-stage build)
├── PaymentService/                  (Stessa struttura)
├── WarehouseService/                (Stessa struttura)
├── docker-compose.yml               (Orchestrazione)
├── .env.example                     (Template variabili)
├── .github/workflows/deploy.yml     (GitHub Actions)
└── README.md                        (Questa guida)
```

## 🚢 Release Management

### Semantic Versioning

Quando effettui un tag:

```bash
# Crea un tag
git tag -a v1.0.0 -m "Release 1.0.0"

# Push del tag
git push origin v1.0.0
```

Il workflow rileverà il tag e creerà immagini con tag `1.0.0`, `1.0`, `latest`.

## 📞 Support

Per problemi:

1. Controlla i log: `docker-compose logs`
2. Verifica i secrets su GitHub
3. Conferma le credenziali DockerHub
4. Controlla la documentazione di GitHub Actions

---

**Ultimo aggiornamento:** Giugno 2026  
**Versione:** 1.0
