# 🔧 Configurazioni Avanzate & Customization

Questa guida ti mostra come customizzare e ottimizzare il deployment per scenari specifici.

## 📋 Sommario

1. [Modificare le Porte](#modificare-le-porte)
2. [Multi-Environment Setup](#multi-environment-setup)
3. [Kubernetes Migration](#kubernetes-migration)
4. [Custom Domains](#custom-domains)
5. [SSL/HTTPS](#ssl-https)
6. [Database Backup/Restore](#database-backup-restore)
7. [Monitoring & Logging](#monitoring-logging)
8. [Performance Tuning](#performance-tuning)
9. [Scaling](#scaling)
10. [Network Configuration](#network-configuration)

---

## 🔌 Modificare le Porte

### Caso: Le porte default sono occupate

**Modifica in `docker-compose.yml`:**

```yaml
services:
  order-api:
    ports:
      - "5011:8080"      # Changed from 5001:8080
      
  warehouse-api:
    ports:
      - "5012:8080"      # Changed from 5002:8080
      
  payment-api:
    ports:
      - "5013:8080"      # Changed from 5003:8080
      
  kafka:
    ports:
      - "9093:9092"      # Changed from 9092:9092
```

**Aggiorna .env:**

```bash
ORDER_SERVICE_PORT=5011
WAREHOUSE_SERVICE_PORT=5012
PAYMENT_SERVICE_PORT=5013
```

---

## 🌍 Multi-Environment Setup

### Scenario: Development, Staging, Production

#### Struttura File:

```
docker-compose.yml              # Base configuration
docker-compose.local.yml        # Local overrides
docker-compose.staging.yml      # Staging overrides
docker-compose.prod.yml         # Production overrides
.env.local
.env.staging
.env.prod
```

#### Comando:

```bash
# Development
docker-compose up -d

# Staging
docker-compose -f docker-compose.yml -f docker-compose.staging.yml up -d

# Production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
env COMPOSE_ENV=prod docker-compose up -d
```

#### Esempio `docker-compose.prod.yml`:

```yaml
version: '3.8'

services:
  order-api:
    restart: always
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - LOG_LEVEL=Information
    deploy:
      resources:
        limits:
          cpus: '1'
          memory: 1024M
        reservations:
          cpus: '0.5'
          memory: 512M

  order-db:
    environment:
      POSTGRES_PASSWORD: ${PROD_DB_PASSWORD}  # More secure
    restart: always

  kafka:
    restart: always
```

---

## ☸️ Kubernetes Migration

### Convertire Docker Compose in Kubernetes

```bash
# Installa kompose
curl -L https://github.com/kubernetes/kompose/releases/download/v1.28.0/kompose-linux-amd64 -o kompose
chmod +x kompose

# Converti
./kompose convert -f docker-compose.yml -o k8s/

# Deployi su Kubernetes
kubectl apply -f k8s/
```

### Manifest Kubernetes di base:

```yaml
# k8s/order-service-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: order-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: order-service
  template:
    metadata:
      labels:
        app: order-service
    spec:
      containers:
      - name: order-service
        image: your-user/warehouse-order-service:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-credentials
              key: order-connection-string
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
        resources:
          requests:
            cpu: 250m
            memory: 512Mi
          limits:
            cpu: 500m
            memory: 1024Mi
---
apiVersion: v1
kind: Service
metadata:
  name: order-service
spec:
  selector:
    app: order-service
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: LoadBalancer
```

---

## 🌐 Custom Domains

### Setup con Nginx Reverse Proxy

**Crea `docker-compose.nginx.yml`:**

```yaml
version: '3.8'

services:
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./certs:/etc/nginx/certs:ro
    networks:
      - warehouse-network
    depends_on:
      - order-api
      - payment-api
      - warehouse-api

networks:
  warehouse-network:
    driver: bridge
```

**Crea `nginx.conf`:**

```nginx
events { worker_connections 1024; }

http {
    upstream order {
        server order-api:8080;
    }
    
    upstream payment {
        server payment-api:8080;
    }
    
    upstream warehouse {
        server warehouse-api:8080;
    }
    
    server {
        listen 80;
        server_name order.localhost;
        
        location / {
            proxy_pass http://order;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
        }
    }
    
    server {
        listen 80;
        server_name payment.localhost;
        
        location / {
            proxy_pass http://payment;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
        }
    }
    
    server {
        listen 80;
        server_name warehouse.localhost;
        
        location / {
            proxy_pass http://warehouse;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
        }
    }
}
```

**Aggiungi /etc/hosts (local testing):**

```
127.0.0.1 order.localhost
127.0.0.1 payment.localhost
127.0.0.1 warehouse.localhost
```

---

## 🔒 SSL/HTTPS

### Let's Encrypt con Certbot

```bash
# Installa Certbot
apt-get install certbot python3-certbot-nginx

# Genera certificati
certbot certonly --standalone -d order.example.com -d payment.example.com -d warehouse.example.com

# Nginx config con HTTPS
```

**`nginx-ssl.conf`:**

```nginx
server {
    listen 443 ssl;
    server_name order.example.com;
    
    ssl_certificate /etc/letsencrypt/live/order.example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/order.example.com/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    
    location / {
        proxy_pass http://order-api:8080;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# Redirect HTTP to HTTPS
server {
    listen 80;
    server_name order.example.com;
    return 301 https://$server_name$request_uri;
}
```

---

## 💾 Database Backup/Restore

### Backup Automatico

**Script `backup.sh`:**

```bash
#!/bin/bash

BACKUP_DIR="/backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

# Backup Order DB
docker exec order-postgres pg_dump -U ecommerce_user order_db | \
  gzip > $BACKUP_DIR/order_db_$TIMESTAMP.sql.gz

# Backup Payment DB
docker exec payment-postgres pg_dump -U ecommerce_user payment_db | \
  gzip > $BACKUP_DIR/payment_db_$TIMESTAMP.sql.gz

# Backup Warehouse DB
docker exec warehouse-postgres pg_dump -U ecommerce_user warehouse_db | \
  gzip > $BACKUP_DIR/warehouse_db_$TIMESTAMP.sql.gz

# Cleanup old backups (keep last 7 days)
find $BACKUP_DIR -name "*.sql.gz" -mtime +7 -delete

echo "Backup completed at $TIMESTAMP"
```

**Cron Job (daily backup):**

```bash
# Edita crontab
crontab -e

# Aggiungi:
0 2 * * * /path/to/backup.sh >> /var/log/backup.log 2>&1
```

### Restore

```bash
# Restore Order DB
gunzip < /backups/order_db_20260601_020000.sql.gz | \
  docker exec -i order-postgres psql -U ecommerce_user order_db

# Oppure
docker exec -i order-postgres pg_restore -U ecommerce_user -d order_db < backup.dump
```

---

## 📊 Monitoring & Logging

### ELK Stack (Elasticsearch, Logstash, Kibana)

**`docker-compose.elk.yml`:**

```yaml
version: '3.8'

services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.0.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
    ports:
      - "9200:9200"

  kibana:
    image: docker.elastic.co/kibana/kibana:8.0.0
    ports:
      - "5601:5601"
    environment:
      - ELASTICSEARCH_HOSTS=http://elasticsearch:9200

  logstash:
    image: docker.elastic.co/logstash/logstash:8.0.0
    volumes:
      - ./logstash.conf:/usr/share/logstash/pipeline/logstash.conf
    ports:
      - "5000:5000"
```

**`logstash.conf`:**

```
input {
  tcp {
    port => 5000
    codec => json
  }
}

filter {
  mutate {
    add_field => { "[@metadata][index_name]" => "logs-%{+YYYY.MM.dd}" }
  }
}

output {
  elasticsearch {
    hosts => ["elasticsearch:9200"]
    index => "%{[@metadata][index_name]}"
  }
}
```

---

## ⚡ Performance Tuning

### Database Connection Pooling

**Program.cs:**

```csharp
services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptions => npgsqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelaySeconds: 5,
                errorCodesToAdd: null)
    )
);

// Connection pooling
services.AddDataProtection()
    .UseCryptographicAlgorithms(
        new AuthenticatedEncryptorConfiguration()
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
        });
```

### Kafka Performance

**`docker-compose.yml` - Kafka tuning:**

```yaml
kafka:
  environment:
    KAFKA_NUM_PARTITIONS: 3
    KAFKA_DEFAULT_REPLICATION_FACTOR: 1
    KAFKA_NUM_RECOVERY_THREADS_PER_DATA_DIR: 3
    KAFKA_LOG_FLUSH_INTERVAL_MESSAGES: 10000
    KAFKA_LOG_FLUSH_INTERVAL_MS: 1000
```

### Redis Caching (Optional)

```yaml
redis:
  image: redis:7-alpine
  ports:
    - "6379:6379"
  volumes:
    - redis_data:/data

volumes:
  redis_data:
```

---

## 🚀 Scaling

### Horizontal Scaling con Docker Compose

```bash
# Scale Order Service a 3 istanze
docker-compose up -d --scale order-api=3

# Scale Payment Service a 2 istanze
docker-compose up -d --scale payment-api=2
```

### Load Balancing

**Con HAProxy:**

```
global
    maxconn 4096

defaults
    mode http
    timeout connect 5000ms
    timeout client 50000ms
    timeout server 50000ms

frontend web
    bind *:80
    default_backend order_servers

backend order_servers
    balance roundrobin
    server order1 order-api-1:8080
    server order2 order-api-2:8080
    server order3 order-api-3:8080
```

---

## 🌐 Network Configuration

### Overlay Network per Multi-Host

```bash
# Crea network overlay
docker network create --driver overlay warehouse_network

# Avvia con network overlay
docker-compose -f docker-compose.yml \
  --network warehouse_network up -d
```

### IPv6 Support

```yaml
networks:
  warehouse_network:
    driver: bridge
    ipam:
      config:
        - subnet: 172.20.0.0/16
        - subnet: "2001:db8:1::/64"
```

---

## 📋 Checklist Customization

- [ ] Porte modificate per il tuo ambiente
- [ ] Multi-environment setup completato
- [ ] Database backup strategy implementato
- [ ] Logging centralizzato configurato
- [ ] Performance tuning completato
- [ ] Scaling strategy definito
- [ ] SSL/HTTPS configurato
- [ ] Custom domains setup
- [ ] Monitoring configurato
- [ ] Disaster recovery plan

---

## 🆘 Troubleshooting Avanzato

### Memoria insufficiente

```bash
# Controlla utilizzo
docker stats

# Modifica limiti in docker-compose.yml
deploy:
  resources:
    limits:
      memory: 2048M
```

### Network Issues

```bash
# Inspeciona network
docker network inspect warehouse_network

# Debug DNS
docker exec order-api nslookup payment-api

# Test connettività
docker exec order-api ping payment-api
```

### Performance Degradation

```bash
# Analizza log
docker-compose logs --tail=100 | grep -i error

# Profiling
docker top <container_id>
docker stats <container_id>
```

---

**Tip:** Molte di queste configurazioni possono essere combinate. Ad esempio, usa ELK + custom domains + SSL per un setup production-ready completo!

---

**Ultima modifica:** Giugno 2026  
**Versione:** 1.0
