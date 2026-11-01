# DEPLOYMENT GUIDE SPECIFICATION

## Fidel Hub — Infrastructure & Release Engineering

| Document Information | Details |
| :--- | :--- |
| **Document** | Deployment Guide Specification |
| **Version** | 1.0-DRAFT |

---

## 1. Container Architecture
- Application containers: `fidelhub-api`, `fidelhub-worker`, `fidelhub-web`.
- Database containers: PostgreSQL 16 Alpine with RLS, Redis 7 Alpine.

## 2. Docker Compose Deployment
```bash
cd infrastructure/docker
docker compose up -d
```
