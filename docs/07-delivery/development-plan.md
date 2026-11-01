# DEVELOPMENT IMPLEMENTATION PLAN

## Fidel Hub — Engineering Execution Blueprint & Live Sprint Tracker

| Document Information | Details |
| :--- | :--- |
| **Document ID** | DEV-001 |
| **Version** | 1.0 |
| **Status** | Active Execution Plan |
| **Current Sprint** | **Sprint 1 (Multi-Tenancy Kernel, Better Auth & Platform Core)** |
| **Current Progress** | **~40% of Sprint 1 Completed (Architecture, Web Shell & Solution CI)** |

---

## 1. Codebase Audit & Current Sprint Status

A comprehensive audit of the repository reveals the current development state:

```
+---------------------------------------------------------------------------------------------------+
| SPRINT 1 AUDIT STATUS (CURRENT SPRINT)                                                             |
+---------------------------------------------------------------------------------------------------+
| Component / Task                                           | Status     | Notes                   |
+---------------------------------------------------------------------------------------------------+
| 1. Documentation & Technical Specifications (15 Modules)   | [DONE 100%]| All 35 docs finalized   |
| 2. Solution Scaffolding & CI Pipeline (FidelHub.sln)       | [DONE 100%]| 15 projects linked + CI |
| 3. React 19 + Vite Web Shell (fidelTheme, Header, Sidebar) | [DONE 100%]| Brand design tokens live|
| 4. ASP.NET Core API Host & Tenancy Primitives (School.Api) | [IN PROG]  | Header-based stub live  |
| 5. PostgreSQL Database Schemas & Kernel RLS Policies       | [PENDING]  | SQL documented, no DB   |
| 6. Better Auth Node.js Service Integration                 | [PENDING]  | Gateway documented      |
| 7. EF Core DbContext & Npgsql Connection Interceptors      | [PENDING]  | Interceptor code drafted|
+---------------------------------------------------------------------------------------------------+
```

### Current Status Conclusion:
We are currently in the middle of **Sprint 1: Multi-Tenancy Kernel, Better Auth & Platform Core**.

---

## 2. Step-by-Step Development Execution Plan

The implementation is broken down into structured, sequential engineering phases:

```mermaid
graph TD
    subgraph Sprint 1: Kernel & Foundation [CURRENT SPRINT]
        S1_1["1.1 Docker Compose Setup (PostgreSQL 16 + Redis)"]
        S1_2["1.2 SQL Schema & RLS Policy Migration Scripts"]
        S1_3["1.3 Better Auth Node.js Service Scaffolding"]
        S1_4["1.4 ASP.NET Core JwtBearer & TenantBranchMiddleware"]
        S1_5["1.5 EF Core 10 DbContext & Connection Interceptor"]
        S1_6["1.6 Verify Tenant Isolation with Testcontainers"]
    end

    subgraph Sprint 2: Student SIS & Calendar
        S2_1["2.1 Ethiopian / Gregorian Dual Calendar Library"]
        S2_2["2.2 Student Aggregate & Guardian Value Objects"]
        S2_3["2.3 Admissions Intake Pipeline & Verification"]
        S2_4["2.4 React Student Directory with Slide-Over Sheet"]
    end

    subgraph Sprint 3: Academics & Attendance
        S3_1["3.1 Timetable Collision Detection Engine"]
        S3_2["3.2 1-Tap Attendance Roster & Outbox Event"]
        S3_3["3.3 Background Worker SMS Dispatch (Ethio Telecom)"]
        S3_4["3.4 Nursery/KG Activity Tracker"]
    end

    subgraph Sprint 4: Student Fees & Invoicing (No GL)
        S4_1["4.1 Fee Structure & Student Snapshot Agreements"]
        S4_2["4.2 Batch Invoice Generator (Draft -> Posted)"]
        S4_3["4.3 Idempotent Payment Collection API (Telebirr/CBE)"]
        S4_4["4.4 Sequential Receipt Generator & PDF Receipt"]
    end

    subgraph Sprint 5: Offline CBT Exam Engine
        S5_1["5.1 Question Bank UI with KaTeX Math"]
        S5_2["5.2 AES-256-GCM Encrypted Exam Packager"]
        S5_3["5.3 Local Exam Venue Server (EVS) with SQLite WAL"]
        S5_4["5.4 Continuous Autosave & Idempotent Batch Sync"]
    end

    subgraph Sprint 6: Reporting & Auxiliary Operations
        S6_1["6.1 Continuous Assessment Gradebook & Lock Deadlines"]
        S6_2["6.2 Dual-Script PDF Report Card Generator (Amharic ፊ)"]
        S6_3["6.3 Library Book Lending & Overdue Fine Settlement"]
        S6_4["6.4 Laboratory Safety Equipment & Room Booking"]
        S6_5["6.5 Student Clinic Dispensary Visit & Parent SMS"]
        S6_6["6.6 Teacher Grade Modification Audit Log"]
    end

    S1_1 --> S1_2 --> S1_3 --> S1_4 --> S1_5 --> S1_6
    S1_6 --> S2_1 --> S2_2 --> S2_3 --> S2_4
    S2_4 --> S3_1 --> S3_2 --> S3_3 --> S3_4
    S3_4 --> S4_1 --> S4_2 --> S4_3 --> S4_4
    S4_4 --> S5_1 --> S5_2 --> S5_3 --> S5_4
    S5_4 --> S6_1 --> S6_2 --> S6_3 --> S6_4 --> S6_5 --> S6_6
```

---

## 3. Sprint 1 Detailed Work Breakdown (To Reach 100% Completion)

### Task 1.1: Local Development Infrastructure
* **Target Directory**: `deploy/` or root `docker-compose.yml`
* **Action**: Create a `docker-compose.yml` defining PostgreSQL 16 with health checks, PgBouncer, and Redis 7.
* **Acceptance Criteria**: Running `docker compose up -d` brings up a healthy PostgreSQL cluster with the 15 logical schemas pre-initialized.

### Task 1.2: Database Migration & Kernel RLS Policies
* **Target Directory**: `backend/FidelHub.Infrastructure/Data/Migrations/`
* **Action**: Author initial EF Core migration or reviewed SQL scripts initializing:
  * `platform.tenants`, `platform.tenant_quotas`
  * `identity.users`, `identity.roles`, `identity.user_branch_access`
  * `school.branches`, `school.academic_years`
  * Standard RLS policies (`app.current_tenant_id`).
* **Acceptance Criteria**: Queries executed as unprivileged `fidel_app_user` automatically filter rows based on `SET LOCAL app.current_tenant_id`.

### Task 1.3: Better Auth Node.js Microservice
* **Target Directory**: `services/auth/`
* **Action**: Initialize lightweight Node.js/Express service using `@better-auth/core`, `@better-auth/organization`, and `@better-auth/jwt`.
* **Acceptance Criteria**: Service exposes `POST /api/auth/sign-in/email` and `GET /api/auth/jwks`, returning cryptographically signed JWTs containing `tenant_id`, `branch_id`, `roles`, and `permissions`.

### Task 1.4: ASP.NET Core Authentication & Middleware
* **Target Directory**: `backend/School.Api/`
* **Action**:
  1. Add `Microsoft.AspNetCore.Authentication.JwtBearer` package to `School.Api.csproj`.
  2. Configure JWT bearer validation against Better Auth JWKS endpoint.
  3. Implement `TenantBranchResolutionMiddleware` extracting claims and validating against authorized branches.
* **Acceptance Criteria**: Unauthenticated requests to protected endpoints return 401; requests with valid Better Auth tokens set `ICurrentTenantContext` and execute successfully.

### Task 1.5: EF Core DbContext & Npgsql Interceptor
* **Target Directory**: `backend/School.Shared/` & `backend/School.Api/`
* **Action**:
  1. Implement `FidelHubDbContext` with global query filters for `ITenantScoped`.
  2. Implement `TenantConnectionInterceptor` executing `SET LOCAL app.current_tenant_id = @tenantId` on every opened database connection/transaction.
* **Acceptance Criteria**: Data queries executed through EF Core automatically respect PostgreSQL RLS boundaries without manual query WHERE clauses.

---

## 4. Definition of Done (DoD) per Sprint

A sprint is considered **100% Done** only when:
1. All user stories in the sprint pass their automated xUnit unit and integration tests.
2. Kernel RLS penetration tests confirm zero cross-tenant data leakage.
3. React components implement the brand theme tokens (`#1B5E3F` and `#E0A526`) with TypeScript strict typing and Zod validation.
4. The GitHub Actions CI workflow ([`.github/workflows/dotnet.yml`](file:///c:/Users/Hana%20PC/Desktop/Projects/Fidel%20Hub/.github/workflows/dotnet.yml)) passes with a green build.
5. All code is merged into `main` and verified.
