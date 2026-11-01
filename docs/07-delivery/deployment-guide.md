# PRODUCTION DEPLOYMENT GUIDE

## Fidel Hub — Infrastructure Topology, Containerization & Operations Manual

| Document Information | Details |
| :--- | :--- |
| **Document ID** | DPG-001 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Target Runtime** | Docker Containers / Kubernetes (EKS / Self-Hosted) |
| **Reverse Proxy** | NGINX / Cloudflare Gateway with TLS 1.3 |
| **Database Pooler** | PgBouncer in Transaction Pooling Mode |

---

## 1. Production Infrastructure Architecture

```mermaid
graph TD
    subgraph Edge & Ingress
        DNS["Cloudflare DNS & WAF (DDoS Shield)"]
        Nginx["NGINX Reverse Proxy / Ingress Controller (TLS 1.3)"]
    end

    subgraph Application Tier
        ApiPod1["ASP.NET Core API Node 1"]
        ApiPod2["ASP.NET Core API Node 2"]
        AuthNode["Better Auth Service (Node.js)"]
        WorkerPod["Background Outbox & Notification Worker"]
    end

    subgraph Persistence & Caching
        PgBouncer["PgBouncer Connection Pooler (Transaction Mode)"]
        PGPrimary["PostgreSQL 16+ Primary Database (Kernel RLS)"]
        PGReplica["PostgreSQL 16+ Read Replica"]
        RedisCluster["Redis Cluster (Distributed Cache & Locks)"]
        S3Storage["S3-Compatible Object Storage (Encrypted Media)"]
    end

    DNS --> Nginx
    Nginx -->|"api.fidelhub.com"| ApiPod1 & ApiPod2
    Nginx -->|"auth.fidelhub.com"| AuthNode
    Nginx -->|"app.fidelhub.com"| StaticWeb["Static React Web CDN"]

    ApiPod1 & ApiPod2 & WorkerPod --> PgBouncer
    AuthNode --> PgBouncer
    PgBouncer --> PGPrimary
    PGPrimary -.->|"Streaming Replication"| PGReplica
    
    ApiPod1 & ApiPod2 --> RedisCluster
    ApiPod1 & ApiPod2 --> S3Storage
```

---

## 2. Docker Compose Production Reference

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: fidel_postgres
    restart: always
    environment:
      POSTGRES_DB: fidel_hub_prod
      POSTGRES_USER: fidel_admin
      POSTGRES_PASSWORD: ${POSTGRES_ADMIN_PASSWORD}
    volumes:
      - pgdata:/var/lib/postgresql/data
      - ./init-schemas.sql:/docker-entrypoint-initdb.d/init.sql
    networks:
      - fidel_backend_net

  pgbouncer:
    image: edoburu/pgbouncer:latest
    container_name: fidel_pgbouncer
    restart: always
    environment:
      DB_USER: fidel_admin
      DB_PASSWORD: ${POSTGRES_ADMIN_PASSWORD}
      DB_HOST: postgres
      DB_PORT: 5432
      DB_NAME: fidel_hub_prod
      POOL_MODE: transaction
      MAX_CLIENT_CONN: 1000
      DEFAULT_POOL_SIZE: 50
    ports:
      - "6432:6432"
    depends_on:
      - postgres
    networks:
      - fidel_backend_net

  redis:
    image: redis:7-alpine
    container_name: fidel_redis
    restart: always
    command: ["redis-server", "--requirepass", "${REDIS_PASSWORD}"]
    volumes:
      - redisdata:/data
    networks:
      - fidel_backend_net

  better-auth:
    image: fidelhub/better-auth:latest
    container_name: fidel_auth
    restart: always
    environment:
      NODE_ENV: production
      PORT: 4000
      DATABASE_URL: postgres://fidel_app_user:${APP_DB_PASSWORD}@pgbouncer:6432/fidel_hub_prod?sslmode=disable
      BETTER_AUTH_SECRET: ${BETTER_AUTH_SECRET}
      BETTER_AUTH_URL: https://auth.fidelhub.com
    depends_on:
      - pgbouncer
    networks:
      - fidel_backend_net

  api:
    image: fidelhub/api:latest
    container_name: fidel_api
    restart: always
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__Postgres: Host=pgbouncer;Port=6432;Database=fidel_hub_prod;Username=fidel_app_user;Password=${APP_DB_PASSWORD};Pooling=false;
      ConnectionStrings__Redis: redis:6379,password=${REDIS_PASSWORD}
      BetterAuth__Issuer: https://auth.fidelhub.com
      BetterAuth__Audience: fidel-hub-api
    depends_on:
      - pgbouncer
      - redis
      - better-auth
    networks:
      - fidel_backend_net

networks:
  fidel_backend_net:
    driver: bridge

volumes:
  pgdata:
  redisdata:
```

---

## 3. Environment Variables & Secret Configuration

| Variable Name | Description | Example / Required Format |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Application environment | `Production` |
| `ConnectionStrings__Postgres` | Npgsql connection string pointing to PgBouncer | `Host=pgbouncer;Port=6432;Database=...` |
| `ConnectionStrings__Redis` | Redis connection endpoint with password | `redis:6379,password=...` |
| `BetterAuth__Issuer` | Authority URL for Better Auth service | `https://auth.fidelhub.com` |
| `BetterAuth__Secret` | 256-bit encryption key for Better Auth sessions | High-entropy 64-character hex string |
| `SMS__EthioTelecom__ApiKey` | Ethio Telecom bulk SMS API credentials | Provisioned telecom gateway key |
| `Finance__Telebirr__MerchantKey`| Telebirr merchant certificate and signing key | Official Ethio Telecom partner token |

---

## 4. Health Checks & Observability Endpoints

* **Liveness Probe** (`/health/live`): Confirms the ASP.NET Core process is responsive. Returns `200 OK`.
* **Readiness Probe** (`/health/ready`): Evaluates connectivity to:
  1. PostgreSQL via PgBouncer (`SELECT 1`).
  2. Redis Cache cluster (`PING`).
  3. Better Auth JWKS endpoint.
  If any dependency fails, returns `503 Service Unavailable`, preventing the ingress load balancer from sending traffic to that instance.
