# SOLUTION STRUCTURE & CODEBASE TOPOLOGY

## Fidel Hub — Repository Layout & Modular Monolith Organization

| Document Information | Details |
| :--- | :--- |
| **Document ID** | STR-001 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |

---

## 1. Top-Level Repository Structure

```text
Fidel Hub/
├── backend/                             # ASP.NET Core Modular Monolith Solution
│   ├── School.Api/                      # REST API Host, Swagger, RLS Interceptors, Middleware
│   ├── School.Shared/                   # Primitives (Result<T>, Auditing, Tenancy Context, Outbox)
│   ├── School.Worker/                   # Background Quartz.NET engine (Outbox processor, Sync worker)
│   ├── Modules/                         # Clean Architecture Domain Modules
│   │   ├── Identity/                    # Auth token consumers, PBAC authorization handlers
│   │   ├── Tenants/                     # School registration, branches, settings
│   │   ├── Students/                    # Student biodata, admissions, guardians, health
│   │   ├── Academics/                   # Terms, classes, timetables, lesson plans, traits
│   │   ├── Attendance/                  # Daily and period attendance tracking
│   │   ├── Examinations/                # Question Bank, CBT papers, offline sync ingestion
│   │   ├── Finance/                     # Fee categories, snapshots, invoices, payments, receipts
│   │   ├── Communication/               # SMS/Push notification gateways
│   │   └── Reporting/                   # Aggregated statistics, MOE compliance exports
│   └── tests/
│       ├── School.ArchitectureTests/    # NetArchTest enforcing modular boundary integrity
│       └── School.UnitTests/            # Domain and business rule test suites
│
├── frontend/
│   ├── web/                             # React + Vite + TypeScript + Material UI
│   │   ├── public/assets/               # Official Fidel Hub brand SVGs & domain icons
│   │   ├── src/theme/                   # Fidel Hub MUI palette & typography tokens
│   │   ├── src/components/              # Shell, Header, Sidebar, Dashboard, CBT Monitor
│   │   └── src/modules/                 # Mirrored module UI views (Academics, CBT, Finance)
│   └── mobile/                          # React Native + Expo + SQLite
│       ├── assets/                      # Mobile app icon and splash graphics
│       └── src/                         # Offline SQLite cache & delta sync engine
│
├── scripts/
│   └── local-exam-server/               # Standalone Node.js + SQLite WAL Local Exam Venue Server
│
├── infrastructure/
│   ├── docker/                          # docker-compose.yml (Postgres, Redis, pgAdmin)
│   └── database/                        # PostgreSQL RLS initialization scripts
│
└── docs/                                # Complete Platform Documentation (01 to 07)
```

---

## 2. Module Boundary Invariants
1. **Forbidden Dependencies**: No module may directly reference the internal persistence or entities of another module.
2. **Public Contracts**: Cross-module communication occurs strictly via public interface contracts (e.g. `IStudentModuleApi`, `IFeeModuleApi`) registered in DI.
3. **Asynchronous Decoupling**: Events spanning multiple domains (e.g. `StudentAbsenceEvent` triggering SMS) are mediated through the Transactional Outbox.
