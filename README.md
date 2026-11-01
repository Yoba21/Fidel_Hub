# Fidel Hub (ፊ)

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Stack: .NET + Postgres + React + React Native](https://img.shields.io/badge/Stack-.NET%20%7C%20Postgres%20%7C%20React%20%7C%20Expo-1B5E3F.svg)](#)
[![Current Sprint: Sprint 1 (In Progress)](https://img.shields.io/badge/Sprint-Sprint%201%20(%7E40%25)-E0A526.svg)](./docs/07-delivery/development-plan.md)

> **Fidel Hub** is an enterprise multi-tenant School Management SaaS platform designed around a **domain-driven modular monolith** architecture, PostgreSQL Row-Level Security (RLS) data isolation, student fee and invoice payment integrity, and offline-first Computer-Based Testing (CBT) synchronization.

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
   Organized into 15 business domain modules (`Students`, `Academics`, `Finance`, `Examinations`, `Attendance`, `Identity`, `Communication`, `Reporting`, `Library`, `Facilities`, `Health`, `Audit`, `Platform`).
2. **Database-Enforced Tenant Isolation**:
   Every tenant-scoped table features `tenant_id` with PostgreSQL Row-Level Security (RLS) and composite foreign keys `(tenant_id, id)`. Standard app queries connect with an unprivileged non-superuser role preventing accidental data leaks.
3. **Student Fee & Payment Integrity**:
   Tuition and school fee transactions are tracked against posted invoices with historical snapshots, idempotency keys, sequential official receipts, and transactional outbox event dispatching (*strictly no general ledger bloat*).
4. **Offline-First CBT & Exam Engine**:
   Air-gapped and low-connectivity campuses utilize the **Local Exam Venue Server (EVS)** with persistent SQLite WAL storage to deliver 100% offline assessments, continuously auto-save progress, and idempotently sync completed sessions to the cloud.

---

## Project Structure

```text
Fidel Hub/
├── FidelHub.sln                         # Root solution linking all 15 backend projects & tests
├── backend/
│   ├── School.Api/                      # ASP.NET Core REST API & Middleware
│   ├── School.Shared/                   # Shared kernel (Result, Auditing, Tenancy Context)
│   ├── School.Worker/                   # Background Quartz.NET worker & Outbox dispatcher
│   ├── Modules/                         # Domain modules (Students, Finance, Academics, etc.)
│   └── tests/                           # Unit, Integration, & NetArchTest architecture tests
├── frontend/
│   ├── web/                             # React 19 + Vite + TypeScript + shadcn/ui + MUI
│   │   ├── public/assets/               # Official Fidel Hub brand SVGs and module icons
│   │   └── src/                         # Fidel Hub theme tokens, header, sidebar & dashboard
│   └── mobile/                          # React Native + Expo + SQLite (WAL mode)
│       └── assets/                      # App icon and mobile assets
├── docs/                                # Canonical 35-document engineering specification suite
│   ├── 01-product/                      # PRD, SRS, Roles & Permissions, Workflows
│   ├── 02-architecture/                 # TAD, ADRs, Solution Structure, Coding Standards
│   ├── 03-database/                     # Database Architecture, Schema, EF Core, ERD, RLS
│   ├── 04-backend/                      # Backend Architecture, API Spec, Auth, Finance
│   ├── 05-frontend-mobile/              # Web App, Mobile App, Offline CBT Synchronization
│   ├── 06-testing/                      # Testing Strategy, Acceptance Criteria, Security Plan
│   └── 07-delivery/                     # Roadmap, Sprint Plan, Development Plan, Deployment
└── .github/workflows/
    └── dotnet.yml                       # Automated CI build and test pipeline for .NET
```

---

## Live Sprint Status & Development Plan

We are currently executing **Sprint 1 (Phase 1: Core Foundation & Multi-Tenancy Kernel)**.

* 📋 **[Full Development Implementation Plan](./docs/07-delivery/development-plan.md)**
* ⏱️ **[Bi-Weekly Sprint Execution Backlog](./docs/07-delivery/sprint-plan.md)**
* 🗺️ **[Product Roadmap & Horizons](./docs/07-delivery/development-roadmap.md)**

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

### 2. Backend Solution (.NET)
```bash
# Build entire solution from repository root
dotnet restore
dotnet build
dotnet test
```

---

## License
MIT License.
