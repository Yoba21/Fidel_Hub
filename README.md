# Fidel Hub (ፊ)

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Stack: .NET + Postgres + React + React Native](https://img.shields.io/badge/Stack-.NET%20%7C%20Postgres%20%7C%20React%20%7C%20Expo-1B5E3F.svg)](#)

> **Fidel Hub** is an enterprise multi-tenant School Management SaaS platform designed around a **domain-driven modular monolith** architecture, PostgreSQL Row-Level Security (RLS) data isolation, student fee and invoice payment integrity, and offline-first mobile synchronization.

---

## Brand Identity & Design System

The visual identity incorporates the Ge'ez character **ፊ** (*Fi* for Fidel / Alphabet), reflecting educational heritage with modern engineering.

- **Deep Green (`#1B5E3F`)**: Primary brand identity, administrative navigation, and high-trust actions.
- **Warm Gold (`#E0A526`)**: Secondary brand accent, achievement indicators, and highlights.
- **Dark Ink (`#1F2933`)**: High-contrast typography and structured content.
- **Off-White (`#FAF7F0`)**: Warm canvas background for comfortable reading.
- **Typography**: `Inter` paired with `Noto Sans Ethiopic` / `Abyssinica SIL`.

---

## Architectural Pillars

1. **Domain-Driven Modular Monolith**:
   Organized by business domains rather than layers (`Students`, `Academics`, `Finance`, `Examinations`, `Attendance`, `Identity`, `Communication`, `Reporting`).
2. **Database-Enforced Tenant Isolation**:
   Every tenant-scoped table features `tenant_id` with PostgreSQL Row-Level Security (RLS) and composite foreign keys `(tenant_id, id)`. Standard app queries connect with a non-superuser role preventing accidental data leaks.
3. **Student Fee & Payment Integrity**:
   Tuition and school fee transactions are tracked against posted invoices with historical snapshots, idempotency keys, sequential official receipts, and transactional outbox event dispatching.
4. **Offline-First CBT & Exam Engine**:
   Mobile and lab clients use React Native + Expo with persistent SQLite storage to deliver offline assessments, auto-save test progress, and sync completed exam sessions reliably.

---

## Project Structure

```text
Fidel Hub/
├── backend/
│   ├── School.Api/                      # ASP.NET Core REST API & Middleware
│   ├── School.Shared/                   # Shared kernel (Result, Auditing, Tenancy Context)
│   ├── School.Worker/                   # Background Quartz.NET worker & Outbox dispatcher
│   ├── Modules/                         # Domain modules (Students, Finance, Academics, etc.)
│   └── tests/                           # Unit, Integration, & NetArchTest architecture tests
├── frontend/
│   ├── web/                             # React + Vite + TypeScript + Material UI
│   │   ├── public/assets/               # Official Fidel Hub brand SVGs and module icons
│   │   └── src/theme/                   # Fidel Hub MUI palette and typography
│   └── mobile/                          # React Native + Expo + SQLite
│       └── assets/                      # App icon and mobile assets
├── infrastructure/
│   ├── docker/                          # Docker Compose (PostgreSQL 16, pgAdmin, Redis)
│   └── database/                        # PostgreSQL RLS initialization scripts
└── documentation/                       # Architecture decisions and specs
```

---

## Getting Started

### Prerequisites
- Node.js (v20+)
- .NET SDK (v8.0 / v9.0 / v10.0)
- Docker & Docker Compose (or local PostgreSQL 16+)

### 1. Web Portal (React + Vite)
```bash
cd frontend/web
npm install
npm run dev
```

### 2. Infrastructure (PostgreSQL & Redis)
```bash
cd infrastructure/docker
docker compose up -d
```

### 3. Backend API
```bash
cd backend/School.Api
dotnet run
```

---

## License
MIT License.
