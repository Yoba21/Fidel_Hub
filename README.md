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
   Architected as a clean modular monolith organized into 15 discrete domain modules (`Tenants`, `Identity`, `Students`, `Admissions`, `Academics`, `Attendance`, `Examinations`, `Grading`, `Finance`, `Communication`, `Reporting`, `Library`, `Facilities`, `Health`, `Audit`, `Platform`) operating inside a unified host API with clear boundary isolation.
2. **Database-Enforced Tenant Isolation & Key Architecture**:
   Every tenant-scoped table features a single-column UUID primary key (`PRIMARY KEY (id)`) paired with a secondary `UNIQUE (tenant_id, id)` constraint (and `UNIQUE (tenant_id, branch_id, id)` where branch-scoped) to enforce composite foreign keys and multi-tenant integrity across all 15 logical PostgreSQL schemas. Kernel-level Row-Level Security (RLS) policies evaluate `app.current_tenant_id` and `app.current_branch_id` on every query executed by the unprivileged `fidel_app_user` database role (`NOSUPERUSER NOBYPASSRLS`).
3. **Student Fee & Payment Integrity**:
   Tuition and school fee transactions are tracked against posted invoices with historical snapshots, idempotency keys, sequential official receipts, and transactional outbox event dispatching (*strictly no general ledger bloat*).
4. **Offline-First CBT & Examination Engine**:
   Air-gapped and low-connectivity campuses utilize the **Local Exam Venue Server (EVS)** with persistent SQLite WAL storage to deliver 100% offline assessments over campus LAN with sub-50ms continuous autosave latency, instant crash recovery, and idempotent batch cloud synchronization. (Standard administrative workflows, SIS, attendance, and fee payments operate online in initial release).

---

## Project Structure

```text
Fidel Hub/
├── FidelHub.sln                         # Root solution linking all 15 backend projects & tests
├── backend/
│   ├── FidelHub.Api/                    # ASP.NET Core 8 REST API, Middleware & RLS Context
│   ├── FidelHub.Shared/                 # Shared kernel (Result<T>, Auditing, ICurrentTenantContext)
│   ├── FidelHub.Worker/                 # Background Quartz.NET worker & Outbox dispatcher
│   ├── Modules/                         # 15 Domain Modules (FidelHub.Modules.*)
│   └── tests/
│       ├── FidelHub.ArchitectureTests/  # NetArchTest modular boundary enforcement
│       ├── FidelHub.UnitTests/          # Unit tests & domain business rule verification
│       └── FidelHub.IntegrationTests/   # Database & PostgreSQL RLS integration tests
├── frontend/
│   ├── web/                             # React 18 + Vite 5 + TypeScript 5 + Material UI (MUI v6)
│   │   ├── public/assets/               # Official Fidel Hub brand SVGs and module icons
│   │   └── src/theme/                   # fidelTheme palette tokens (#1B5E3F, #E0A526)
│   └── mobile/                          # React Native + Expo + SQLite (WAL mode)
│       └── assets/                      # App icon and mobile assets
├── scripts/
│   └── local-exam-server/               # Standalone Local Exam Venue Server (EVS) for offline CBT
├── docs/                                # Canonical 35-document engineering specification suite
│   ├── 01-product/                      # PRD, SRS, Roles & Permissions, Workflows
│   ├── 02-architecture/                 # TAD, ADRs, Solution Structure, Coding Standards
│   ├── 03-database/                     # Database Architecture, Schema, EF Core, ERD, RLS
│   ├── 04-backend/                      # Backend Architecture, API Spec, Auth, Finance
│   ├── 05-frontend-mobile/              # Web App, Mobile App, Offline CBT Synchronization
│   ├── 06-testing/                      # Testing Strategy, Acceptance Criteria, Security Plan
│   └── 07-delivery/                     # Roadmap, Sprint Plan, Development Plan, Deployment
└── .github/workflows/
    └── dotnet.yml                       # Automated CI build and test pipeline for .NET 8
```

---

## Technical Specification & Stack Standards

| Layer / Component | Technology Specification | Standard Version |
| :--- | :--- | :--- |
| **Backend Runtime** | .NET (LTS) | `.NET 10 (LTS)` |
| **Target Framework** | C# Target Framework Moniker | `net10.0` |
| **Web API Framework** | ASP.NET Core Minimal APIs | `Microsoft.AspNetCore.*: 10.x` |
| **ORM / Data Access** | Entity Framework Core & Dapper | `Microsoft.EntityFrameworkCore.*: 10.x` |
| **Container Runtime Image** | Microsoft Container Registry (ASP.NET) | `mcr.microsoft.com/dotnet/aspnet:10.0` |
| **Container SDK Image** | Microsoft Container Registry (SDK) | `mcr.microsoft.com/dotnet/sdk:10.0` |
| **Database** | PostgreSQL with Row-Level Security (RLS) | `PostgreSQL 16+` |
| **Frontend Portal** | React 19 + TypeScript + Vite + shadcn/ui | `Node.js 20+ LTS` |
| **Mobile Application** | React Native + Expo + SQLite (WAL mode) | `Expo SDK 51+` |

---

## Live Sprint Status & Development Plan

We are currently executing **Sprint 1 (Phase 1: Core Foundation & Multi-Tenancy Kernel)**.

* 📋 **[Full Development Implementation Plan](./docs/07-delivery/development-plan.md)**
* ⏱️ **[Bi-Weekly Sprint Execution Backlog](./docs/07-delivery/sprint-plan.md)**
* 🗺️ **[Product Roadmap & Horizons](./docs/07-delivery/development-roadmap.md)**

---

## Getting Started

### Prerequisites
- Developers must install .NET 10 SDK or higher.
- Node.js (v20+ LTS)
- Docker & Docker Compose (or local PostgreSQL 16+)

### Container Guidelines (Docker / CI-CD)
When containerizing Fidel Hub services, use the standardized .NET 10 LTS container images:
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:10.0`
- **Runtime Base Image**: `mcr.microsoft.com/dotnet/aspnet:10.0`

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
