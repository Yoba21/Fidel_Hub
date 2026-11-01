# SOLUTION STRUCTURE & CODEBASE TOPOLOGY

## Fidel Hub — Repository Layout & Modular Monolith Organization

| Document Information | Details |
| :--- | :--- |
| **Document ID** | STR-001 |
| **Version** | 1.1 |
| **Status** | Approved Baseline |
| **Target Framework** | .NET 10 LTS (`net10.0`) / ASP.NET Core 10 / EF Core 10 |

---

## 1. Top-Level Repository Structure

```text
Fidel Hub/
├── FidelHub.sln                             # Unified Visual Studio / dotnet Solution
│
├── backend/                                 # ASP.NET Core 10 Modular Monolith Solution
│   ├── FidelHub.Api/                        # REST API Host, Swagger, RLS Interceptors, Middleware
│   │   ├── Controllers/                     # Thin HTTP route mappings & Swagger metadata
│   │   ├── Middleware/                      # TenantBranchResolutionMiddleware, ProblemDetailsMiddleware
│   │   ├── Filters/                         # Action and exception filters
│   │   └── Program.cs                       # App startup, DI composition root, module discovery
│   │
│   ├── FidelHub.Shared/                     # Shared Kernel & Cross-Cutting Infrastructure
│   │   ├── Kernel/                          # ITenantScoped, IBranchScoped, IAuditableEntity, BaseEntity<TId>
│   │   ├── Context/                         # ICurrentTenantContext & ambient resolution
│   │   ├── Events/                          # IDomainEvent, DomainEventNotification
│   │   ├── Primitives/                      # Result<T>, Error, ValueObject
│   │   └── Infrastructure/                  # Multi-tenant connection interceptors, OutboxMessage entity
│   │
│   ├── FidelHub.Worker/                     # Background Quartz.NET engine (Outbox processor, Sync worker)
│   │   ├── Jobs/                            # OutboxProcessorJob, TruancyAlertJob, ReportCompileJob
│   │   └── Program.cs                       # Worker DI composition & background job scheduling
│   │
│   ├── Modules/                             # 15 Business Domain Modules (16 Vertical Slice Projects)
│   │   ├── FidelHub.Modules.Tenants/        # 01: School organizations, branches, calendar configuration
│   │   ├── FidelHub.Modules.Identity/       # 02: Better Auth JWT consumer, user branch access, PBAC policies
│   │   ├── FidelHub.Modules.Students/       # 03a: Tenant-owned student master SIS biodata, Ethiopian names, guardians
│   │   ├── FidelHub.Modules.Admissions/     # 03b: Online intake applications, document verification, enrollment
│   │   ├── FidelHub.Modules.Academics/      # 04: Dual calendars (EC/GC), terms, classes, timetables, lesson plans
│   │   ├── FidelHub.Modules.Attendance/     # 05: Homeroom daily & period-by-period attendance tracking
│   │   ├── FidelHub.Modules.Examinations/   # 06: Question Bank (KaTeX), CBT papers, offline sync ingestion
│   │   ├── FidelHub.Modules.Grading/        # 07: Continuous Assessment, spreadsheet marksheet, term report cards
│   │   ├── FidelHub.Modules.Finance/        # 08: Fee structures, snapshot agreements, invoices, payments (No GL)
│   │   ├── FidelHub.Modules.Communication/  # 09: SMS gateway (Ethio Telecom/Telebirr), system announcements
│   │   ├── FidelHub.Modules.Reporting/      # 10: Transcripts, Ge'ez Amharic PDF report cards, MoE statistics
│   │   ├── FidelHub.Modules.Library/        # 11: Physical & digital cataloging, circulation, overdue fine engine (Phase 2)
│   │   ├── FidelHub.Modules.Facilities/     # 12: Laboratory apparatus, chemical stocks, room/lab bookings (Phase 2)
│   │   ├── FidelHub.Modules.Health/         # 13: Student medical profiles, clinic dispensary visits, incident logs (Phase 2)
│   │   ├── FidelHub.Modules.Audit/          # 14: Tamper-evident grade modifications & administrative activity logs (Phase 2)
│   │   └── FidelHub.Modules.Platform/       # 15: Platform SuperAdmin SaaS CRM, subscription tiers, student headcount quotas (Phase 2)
│   │
│   └── tests/
│       ├── FidelHub.ArchitectureTests/      # NetArchTest enforcing vertical slice boundaries & no cyclic refs
│       ├── FidelHub.UnitTests/              # Domain invariant unit tests (Value objects, entities, calculation logic)
│       └── FidelHub.IntegrationTests/       # Testcontainers PostgreSQL RLS integration tests
│
├── frontend/
│   ├── web/                                 # React 18 + Vite 5 + TypeScript 5 + Material UI (MUI v6)
│   │   ├── public/assets/                   # Official Fidel Hub brand SVGs & domain icons
│   │   ├── src/theme/                       # fidelTheme palette tokens (#1B5E3F, #E0A526, #FAF7F0, #1F2933)
│   │   ├── src/components/                  # Shell, Header, Sidebar, Dashboard, CBT Monitor
│   │   └── src/modules/                     # Mirrored module UI views (Academics, CBT, Finance, Students, etc.)
│   └── mobile/                              # React Native + Expo + SQLite (WAL mode)
│       ├── assets/                          # Mobile app icon and splash graphics
│       └── src/                             # Screen views & SQLite read caching
│
├── scripts/
│   └── local-exam-server/                   # Standalone Node.js + SQLite WAL Local Exam Venue Server (EVS)
│
├── infrastructure/
│   ├── docker/                              # docker-compose.yml (PostgreSQL 16+, Redis, pgAdmin)
│   └── database/                            # PostgreSQL RLS initialization scripts & migrations
│
└── docs/                                    # Complete Platform Documentation Suite (01 to 07)
```

---

## 2. Module Boundary Invariants

1. **Forbidden Dependencies**: No module may directly reference the internal persistence or entities of another module. Cross-module database writes are strictly prohibited.
2. **Public Contracts**: Cross-module communication occurs strictly via public interface contracts (e.g. `IStudentModuleApi`, `IFeeModuleApi`) registered in DI.
3. **Asynchronous Decoupling**: Events spanning multiple domains (e.g. `StudentAbsenceEvent` triggering SMS notification) are mediated through the Transactional Outbox pattern.
4. **Architectural Verification**: NetArchTest suites in `FidelHub.ArchitectureTests` enforce boundary invariants on every build and CI pull request.
