# 🏭 Warehouse Application - Microservizi E-Commerce

**Una architettura moderna di microservizi per un'applicazione e-commerce con orchestrazione Docker e CI/CD automatico su DockerHub.**

## 📊 Overview

Questa applicazione implementa un sistema e-commerce basato su **3 microservizi indipendenti** con:

- ✅ **Architettura a microservizi** - Servizi decoupled e scalabili
- ✅ **Event-Driven Architecture** - Comunicazione via Kafka
- ✅ **CI/CD Automatico** - GitHub Actions → DockerHub
- ✅ **Database Isolation** - PostgreSQL per servizio
- ✅ **Docker Compose** - Orchestrazione locale e deployment
- ✅ **Multi-stage Docker Builds** - Immagini ottimizzate (~150MB)
- ✅ **Fully Documented** - Guida completa + architettura + troubleshooting

## 🎯 Microservizi

| Servizio | Porta | DB | Descrizione |
|----------|-------|----|-|
| **Order Service** | 5001 | order_db | Gestisce ordini e ordini |
| **Payment Service** | 5003 | payment_db | Elabora pagamenti |
| **Warehouse Service** | 5002 | warehouse_db | Gestisce inventario e stock |

Tutti communicano tramite **Kafka** per event decoupling.

## 🚀 Quick Start

```bash
# 1. Configurazione
cp .env.example .env
nano .env  # Modifica con i tuoi credentials

# 2. Build e avvia
docker-compose build
docker-compose up -d

# 3. Verifica
./deploy.sh health
```

👉 **Leggi [QUICK_START.md](QUICK_START.md) per una guida completa in 5 minuti**

## 📁 Struttura Progetti

Ogni microservizio ha la stessa struttura pulita:

```
ServiceName/
├── ServiceName.WebApi/          # REST API entry point
│   ├── Program.cs              # DI container setup
│   ├── appsettings.json        # Configurazione
│   └── Controllers/            # REST endpoints
│
├── ServiceName.Business/        # Business logic
│   └── IServiceInterface.cs   # Contracts
│
├── ServiceName.Repository/      # Data Access Layer
│   ├── DbContext.cs           # Entity Framework
│   └── Entities/
│
├── ServiceName.Shared/          # Shared DTOs
│   ├── Requests/
│   └── Events/
│
├── ServiceName.ClientHttp/      # HTTP Client
│   └── ServiceClient.cs       # External calls
│
└── Dockerfile                   # Multi-stage build (4 stage)
```

## 🔄 CI/CD Pipeline

### Automatic Workflow: `.github/workflows/deploy.yml`

**Trigger:** Push su `main` o `develop`

```
Git Push
  ↓
GitHub Detects Changes
  ↓
GitHub Actions (Parallel Build)
  ├─ Build Order Service
  ├─ Build Payment Service
  └─ Build Warehouse Service
  ↓
DockerHub Push
  ├─ docker.io/your-user/warehouse-order-service:latest
  ├─ docker.io/your-user/warehouse-payment-service:latest
  └─ docker.io/your-user/warehouse-warehouse-service:latest
```

**Vantaggi:**
- ✓ Build parallelo (3x più veloce)
- ✓ Tagging automatico (latest, branch, sha, version)
- ✓ Caching di Docker per rebuild più veloci
- ✓ Deploy ready - immagini su DockerHub

## 📋 Infrastruttura Locale

```yaml
7 Containers Orchestrati:
├─ 3 × Microservizi (.NET 8)
├─ 3 × PostgreSQL Databases
├─ 1 × Kafka Message Broker
└─ Networking interno automatico
```

**Volumi persistenti:**
- `order_db_data` - Order Service Database
- `payment_db_data` - Payment Service Database
- `warehouse_db_data` - Warehouse Service Database

## 🔐 Setup GitHub & DockerHub

### 1. Crea DockerHub Repositories

```bash
# Nel tuo account DockerHub, crea:
warehouse-order-service
warehouse-payment-service
warehouse-warehouse-service
```

### 2. GitHub Secrets

Va a **Repository → Settings → Secrets → Actions**:

```
DOCKERHUB_USERNAME     = your_dockerhub_username
DOCKERHUB_TOKEN        = your_dockerhub_access_token
GITHUB_TOKEN           = ghp_your_personal_access_token (read:packages)
```

### 3. .env Locale

```bash
cp .env.example .env
# Modifica .env con i tuoi credentials
DOCKERHUB_USERNAME=your_username
DOCKERHUB_TOKEN=your_token
GITHUB_TOKEN=your_github_pat
```

## 📦 Build & Deploy

### Build Locale

```bash
# Build tutti
docker-compose build

# Build specifico
docker-compose build order-api
```

### Avvia Locale

```bash
# Avvia tutto
docker-compose up -d

# Visualizza log
docker-compose logs -f

# Ferma
docker-compose down
```

### Push su DockerHub

```bash
# Automatico via GitHub Actions
git push origin main

# O manuale
./deploy.sh push
```

## 🧪 Verifica Deployment

```bash
# Health check
./deploy.sh health

# Curl endpoints
curl http://localhost:5001/health  # Order
curl http://localhost:5003/health  # Payment
curl http://localhost:5002/health  # Warehouse

# Log
docker-compose logs -f order-api
```

## 📚 Documentazione Completa

| File | Descrizione |
|------|-----------|
| [QUICK_START.md](QUICK_START.md) | ⚡ Avvio in 5 minuti |
| [DEPLOYMENT.md](DEPLOYMENT.md) | 📖 Guida completa deployment |
| [ARCHITECTURE.md](ARCHITECTURE.md) | 🏗️ Schemi architetturali |
| [SETUP_SUMMARY.md](SETUP_SUMMARY.md) | ✅ Checklist setup |
| [COMMANDS.md](COMMANDS.md) | 💻 Comandi comuni |

## 🛠️ Script Helper

```bash
# Uso interattivo
./deploy.sh

# Opzioni specifiche
./deploy.sh check     # Verifica prerequisiti
./deploy.sh setup     # Setup .env
./deploy.sh build     # Build servizi
./deploy.sh start     # Avvia servizi
./deploy.sh stop      # Ferma servizi
./deploy.sh logs      # Visualizza log
./deploy.sh health    # Health check
./deploy.sh push      # Push a DockerHub
./deploy.sh clean     # Cleanup
```

## 🔗 Porte Esposte

| Servizio | Host Port | Container | Uso |
|----------|-----------|-----------|-----|
| Order API | 5001 | 8080 | REST API |
| Warehouse API | 5002 | 8080 | REST API |
| Payment API | 5003 | 8080 | REST API |
| Order DB | 5431 | 5432 | PostgreSQL |
| Warehouse DB | 5432 | 5432 | PostgreSQL |
| Payment DB | 5433 | 5432 | PostgreSQL |
| Kafka | 9092 | 9092 | Message Broker |

## 🔄 Architettura Event-Driven

```
Order Service (5001)
  └─ Pubblica: OrderCreated, OrderUpdated
       ↓ (Kafka Topic: orders)
       ├→ Payment Service (5003)
       │   └─ Ascolta: OrderCreated
       │   └─ Pubblica: PaymentProcessed
       │        ↓ (Kafka Topic: payments)
       │        └→ Warehouse Service (5002)
       │
       └→ Warehouse Service (5002)
           └─ Ascolta: OrderCreated, PaymentProcessed
           └─ Pubblica: StockReserved
```

## 🐘 Database Management

```bash
# Accedi al database Order
docker exec -it order-postgres psql -U ecommerce_user -d order_db

# Query da shell
docker exec -it order-postgres psql -U ecommerce_user -d order_db -c "SELECT version();"

# Backup
docker exec order-postgres pg_dump -U ecommerce_user order_db > backup.sql
```

## 🔍 Troubleshooting

**Servizi non partono:**
```bash
docker-compose logs
lsof -i :5001
```

**Errore NuGet:**
```bash
echo $GITHUB_TOKEN
docker-compose build --no-cache
```

**Porta occupata:**
```bash
lsof -i :5001
kill -9 <PID>
```

👉 **Vedi [DEPLOYMENT.md](DEPLOYMENT.md) sezione "Troubleshooting"** per soluzioni complete

## 📊 Tech Stack

- **Runtime:** .NET 8 (Alpine Linux)
- **Databases:** PostgreSQL 15
- **Message Broker:** Apache Kafka 7.4.0
- **Container Orchestration:** Docker Compose v3.8
- **CI/CD:** GitHub Actions
- **Registry:** DockerHub
- **Package Manager:** NuGet (con GitHub Packages support)

## 🎯 Best Practices Implementati

✅ Clean Architecture (Business/Repository/Shared layers)  
✅ Event-Driven Communication (Kafka)  
✅ Database per Service (Polyglot persistence)  
✅ Multi-stage Docker builds (Immagini ~150MB)  
✅ Environment-based configuration (.env)  
✅ Health checks endpoints  
✅ Comprehensive logging  
✅ Infrastructure as Code (docker-compose.yml)  
✅ Automated CI/CD pipeline  
✅ Documentation complete  

## 🚀 Deployment Options

### Option 1: Local Development
```bash
docker-compose up -d
```

### Option 2: DockerHub + Docker Compose
```bash
export $(cat .env | grep -v '#' | xargs)
docker-compose pull
docker-compose up -d
```

### Option 3: Kubernetes (Future)
```bash
# Convertibile con kompose
kompose convert -f docker-compose.yml
kubectl apply -f docker-compose-services.yaml
```

## 📞 Support & Troubleshooting

1. Leggi [QUICK_START.md](QUICK_START.md) per setup rapido
2. Consulta [DEPLOYMENT.md](DEPLOYMENT.md) per guide complete
3. Vedi [ARCHITECTURE.md](ARCHITECTURE.md) per schemi
4. Usa [COMMANDS.md](COMMANDS.md) per comandi comuni
5. Esegui `./deploy.sh` per help interattivo

## 📝 Changelog

### v1.0 (Giugno 2026)
- ✅ Initial setup con 3 microservizi
- ✅ GitHub Actions CI/CD pipeline
- ✅ DockerHub integration
- ✅ Docker Compose orchestration
- ✅ Complete documentation

## 📄 License

MIT License - Progetto Universitario

---

**Ultimo aggiornamento:** Giugno 2026  
**Status:** ✅ Production Ready  
**Mantainer:** Federico Becchi
