# SOLUTION STRUCTURE & REPOSITORY LAYOUT

## Fidel Hub — Codebase Structure Guide

| Document Information | Details |
| :--- | :--- |
| **Document** | Solution Structure Specification |
| **Version** | 1.0-DRAFT |
| **Status** | Template / Specification Baseline |

---

## 1. Directory Tree Overview

```text
Fidel Hub/
├── backend/
│   ├── School.Api/                      # REST API Host, Swagger, Middleware
│   ├── School.Shared/                   # Cross-cutting primitives (Result, Auditing, Tenancy)
│   ├── School.Worker/                   # Background job runner (Quartz.NET, Outbox Consumer)
│   ├── Modules/
│   │   ├── Identity/                    # Authentication, RBAC, Claims
│   │   ├── Tenants/                     # School registration, branches, settings
│   │   ├── Students/                    # Student records, guardians, profiles
│   │   ├── Admissions/                  # Application intake, enrollment workflows
│   │   ├── Academics/                   # Academic years, terms, classes, subjects
│   │   ├── Attendance/                  # Daily/period attendance tracking
│   │   ├── Examinations/                # Exam schedules, marks, CBT, report cards
│   │   ├── Finance/                     # Invoices, fee structures, payments, receipts
│   │   ├── Communication/               # SMS/Email gateways, notifications, SignalR
│   │   └── Reporting/                   # Aggregated statistics, exports
│   └── tests/
│       ├── School.ArchitectureTests/    # NetArchTest enforcing modular monolith rules
│       ├── School.UnitTests/            # Isolated unit test suites
│       └── School.IntegrationTests/     # Testcontainers PostgreSQL tests
│
├── frontend/
│   ├── web/                             # React + Vite + TypeScript + MUI
│   │   ├── public/assets/               # SVG Brand assets and feature icons
│   │   ├── src/theme/                   # Fidel Hub MUI palette and typography
│   │   ├── src/components/              # Shell, Header, Sidebar, Dashboard
│   │   └── src/modules/                 # Mirrored web UI module screens
│   │
│   └── mobile/                          # React Native + Expo + SQLite
│       ├── assets/                      # App icons and mobile graphics
│       └── src/                         # Local database & offline sync engine
│
├── infrastructure/
│   ├── docker/                          # docker-compose.yml (Postgres, pgAdmin, Redis)
│   └── database/                        # Migration scripts & RLS setup
│
└── documentation/                       # Comprehensive platform docs (01 to 07)
```

---

## 2. Module Internal Architecture

Each module under `backend/Modules/<ModuleName>` strictly follows Clean Architecture:

```
Modules/<ModuleName>/
├── Domain/
│   ├── Entities/                        # Rich domain entities with private setters
│   ├── ValueObjects/                    # Immutable value objects
│   ├── Events/                          # Domain events
│   └── Exceptions/                      # Domain-specific business rule exceptions
│
├── Application/
│   ├── Commands/                        # Write operations (CQRS)
│   ├── Queries/                         # Read operations (CQRS)
│   ├── DTOs/                            # Data transfer contracts
│   └── Validators/                      # FluentValidation rules
│
└── Infrastructure/
    ├── Persistence/                     # Module DbContext, EntityConfigurations
    └── Services/                        # External adapters and implementations
```
