# DEVELOPMENT ROADMAP SPECIFICATION

## Fidel Hub — Phased Product Milestones

| Document Information | Details |
| :--- | :--- |
| **Document** | Development Roadmap Specification |
| **Version** | 1.0-DRAFT |

---

## Roadmap Phases

```mermaid
flowchart LR
    P1["Phase 1: Foundation & RLS"] --> P2["Phase 2: Identity & SIS"]
    P2 --> P3["Phase 3: Academics & Attendance"]
    P3 --> P4["Phase 4: Fee & Invoicing"]
    P4 --> P5["Phase 5: Exams & Offline CBT"]
    P5 --> P6["Phase 6: Reporting & Production Launch"]
```

### Phase 1: Foundation, Multi-Tenancy & Scaffolding
- PostgreSQL Row-Level Security, project architecture, and brand design tokens.

### Phase 2: Identity, Tenancy & Student Information System
- Multi-tenant JWT auth, school onboarding, student registration, and guardian records.

### Phase 3: Academic Setup & Attendance Tracking
- Dual calendar (Ethiopian + Gregorian), period/daily attendance, SMS alerts.

### Phase 4: Fee Management & Invoicing
- Fee schedules, invoices, payments, receipts, idempotency protection.

### Phase 5: Examinations, Grading & Offline CBT Engine
- Assessments, CBT offline test taking, rankings, PDF report cards, and Expo SQLite sync.

### Phase 6: Reporting, Analytics & Pilot School Launch
- Aggregate dashboards, Ministry of Education export formats, performance testing, and production deployment.
