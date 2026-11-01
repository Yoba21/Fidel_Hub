# SPRINT DELIVERY PLAN SPECIFICATION

## Fidel Hub — Bi-Weekly Engineering Sprints & Execution Backlog

| Document Information | Details |
| :--- | :--- |
| **Document ID** | SPL-001 |
| **Version** | 1.1 (Updated with Live Sprint Progress Tracking) |
| **Status** | Approved Baseline |
| **Current Sprint** | **Sprint 1: Multi-Tenancy Kernel, Better Auth & Platform Core (~40% Complete)** |
| **Detailed Plan** | See [`development-plan.md`](./development-plan.md) for task-by-task breakdown |

---

## 1. Sprint Cadence & Velocity Framework

Each sprint runs on a strict two-week iteration cycle:
* **Day 1**: Sprint Planning & Story Point Allocation.
* **Days 2–8**: Vertical Slice Implementation (Domain Aggregates + EF Core + MediatR Handlers + React UI).
* **Day 9**: Cross-Module Integration, RLS Penetration Testing & Load Benchmarking.
* **Day 10**: Sprint Review, End-of-Sprint Demo & Retrospective.

---

## 2. Live Sprint Status & Progress Matrix

| Sprint | Horizon / Focus | Target Window | Status | Completion % |
| :--- | :--- | :--- | :---: | :---: |
| **Sprint 1** | **Multi-Tenancy Kernel, Better Auth & Platform Core** | **Nov 1 – Nov 14, 2026** | **ACTIVE** | **~40%** |
| **Sprint 2** | Student SIS, Online Admissions & Dual Calendar | Nov 15 – Nov 28, 2026 | Scheduled | 0% |
| **Sprint 3** | Academic Timetables, Attendance & Automated SMS | Nov 29 – Dec 12, 2026 | Scheduled | 0% |
| **Sprint 4** | Student Fee Management & Invoicing (No GL) | Dec 13 – Dec 26, 2026 | Scheduled | 0% |
| **Sprint 5** | Offline CBT Local Venue Server & Question Bank | Dec 27 – Jan 09, 2027 | Scheduled | 0% |
| **Sprint 6** | Gradebooks, Dual-Script Report Cards & Auxiliary Ops | Jan 10 – Jan 23, 2027 | Scheduled | 0% |

---

## 3. Detailed 6-Sprint Execution Backlog

### Sprint 1: Multi-Tenancy Kernel, Better Auth & Platform Core (CURRENT SPRINT)
* **Sprint Goal**: Establish rock-solid database kernel RLS isolation, solution CI, and the Better Auth gateway.
* **Stories & Tasks**:
  * [x] *STORY-00*: Architecture & 15-Module Documentation Suite (Completed).
  * [x] *STORY-00B*: Root Solution Scaffolding (`FidelHub.sln`) & GitHub Actions CI Workflow (Completed).
  * [x] *STORY-04*: Scaffold React 19 + Vite web portal with Fidel Hub brand design system (Completed).
  * [ ] *STORY-01*: Configure PostgreSQL 16+ with 15 schemas and base tenant isolation policies (Pending).
  * [ ] *STORY-02*: Deploy Better Auth Node service with `@better-auth/organization` and JWT plugins (Pending).
  * [ ] *STORY-03*: Implement ASP.NET Core `TenantBranchResolutionMiddleware` and connection interceptors (In Progress - 30%).
* **Deliverable**: Authenticated multi-tenant shell verifying that Tenant A cannot query Tenant B data.

### Sprint 2: Student SIS, Online Admissions & Dual Calendar
* **Sprint Goal**: Complete the student lifecycle and the Ethiopian/Gregorian dual calendar engine.
* **Stories & Tasks**:
  * [ ] *STORY-05*: Build Ethiopian Calendar (EC) / Gregorian Calendar (GC) date conversion library.
  * [ ] *STORY-06*: Implement Student Information System (SIS) supporting three-part Ethiopian names.
  * [ ] *STORY-07*: Develop Admissions Intake Pipeline (draft, review, screening, approval).
  * [ ] *STORY-08*: Build Student Directory UI with high-density data tables and slide-over inspector.
* **Deliverable**: Complete admissions intake and student profile management.

### Sprint 3: Academic Timetables, Attendance & Automated Truancy SMS
* **Sprint Goal**: Deploy classroom scheduling, 1-tap attendance, and asynchronous SMS outbox.
* **Stories & Tasks**:
  * [ ] *STORY-09*: Develop Timetable Scheduling Grid with automated teacher/room collision detection.
  * [ ] *STORY-10*: Build Rapid Attendance Roster UI (1-click mark present, exception toggles).
  * [ ] *STORY-11*: Implement Transactional Outbox pattern and background worker engine.
  * [ ] *STORY-12*: Integrate Ethio Telecom / Telebirr SMS gateways for automated truancy dispatch.
  * [ ] *STORY-12B*: Build Nursery / Kindergarten daily developmental activity tracker.
* **Deliverable**: Automated attendance taking with SMS dispatched to parents within 15 minutes.

### Sprint 4: Student Fee Management & Idempotent Payments (No GL)
* **Sprint Goal**: Implement the high-integrity student fee billing engine.
* **Stories & Tasks**:
  * [ ] *STORY-13*: Configure branch fee structures and immutable student fee agreement snapshots.
  * [ ] *STORY-14*: Implement batch invoice generation and the `DRAFT` → `POSTED` state machine.
  * [ ] *STORY-15*: Build payment collection API with mandatory UUID `Idempotency-Key` header.
  * [ ] *STORY-16*: Develop sequential receipt number generator and printable official PDF receipts.
  * [ ] *STORY-17*: Build Cashier Portal in React with live balance calculation.
* **Deliverable**: Complete student billing and payment collection with zero double-charging risk.

### Sprint 5: Offline CBT Local Venue Server & Question Bank
* **Sprint Goal**: Build the resilient 100% offline Computer-Based Testing engine.
* **Stories & Tasks**:
  * [ ] *STORY-18*: Build Question Bank UI supporting KaTeX formulas, difficulty tags, and bulk ZIP import.
  * [ ] *STORY-19*: Implement AES-256 encrypted exam package exporter with SHA-256 checksums.
  * [ ] *STORY-20*: Develop Local Exam Venue Server (EVS) with time-locked decryption.
  * [ ] *STORY-21*: Implement sub-50ms continuous autosave in Local SQLite (WAL mode).
  * [ ] *STORY-22*: Build idempotent cloud batch sync endpoint (`ON CONFLICT DO NOTHING`).
* **Deliverable**: Functional offline exam delivery system resilient to total internet dropouts.

### Sprint 6: Gradebooks, Report Cards & Auxiliary School Operations
* **Sprint Goal**: Finalize term reporting, dual-script report cards, and auxiliary facility modules.
* **Stories & Tasks**:
  * [ ] *STORY-23*: Implement spreadsheet Continuous Assessment marksheet with term lock deadlines.
  * [ ] *STORY-24*: Develop batch PDF generator for dual-script report cards with verification QR codes.
  * [ ] *STORY-25*: Implement Library circulation desk (cataloging, loans, overdue fine accrual).
  * [ ] *STORY-26*: Implement Laboratory equipment registry and collision-free room booking.
  * [ ] *STORY-27*: Build Student Clinic Dispensary intake with emergency guardian triage alerts.
  * [ ] *STORY-28*: Implement Staff/Teacher mark modification audit trail (`audit.teacher_access_logs`).
* **Deliverable**: Complete institutional platform ready for pilot school onboarding.
