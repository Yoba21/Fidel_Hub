# SOFTWARE REQUIREMENTS SPECIFICATION (SRS)

## Fidel Hub — School Management SaaS Platform

| Document Information | Details |
| :--- | :--- |
| **Document** | Software Requirements Specification (SRS) |
| **Version** | 1.0-DRAFT |
| **Status** | Template / Specification Baseline |
| **Target Audience** | Engineering, QA, Product Management |
| **Market** | Ethiopia (K–12 Multi-Tenant Institutions) |

---

## 1. Introduction

### 1.1 Purpose
This document specifies the software requirements for Fidel Hub, defining functional, non-functional, interface, and operational specifications for the platform across Web, Mobile, and Backend API layers.

### 1.2 Scope
- **Core SaaS**: Multi-tenant institutional management, branch-level administration, user identity & access control.
- **Academic & Operational**: Student lifecycle, academic calendar (Ethiopian + Gregorian), period/daily attendance, examinations, and grading.
- **Financial**: Student invoicing, fee collections, payment reconciliation, and double-entry ledger.
- **Communication & Reporting**: Multi-channel notifications (SMS/Push), official transcripts, and Ministry of Education standard reports.

### 1.3 Definitions, Acronyms, and Abbreviations
- **Tenant**: An independent school organization (e.g., St. Joseph Academy).
- **Branch**: An operational sub-unit of a school (e.g., Kera Campus, Sarbet Campus).
- **RLS**: PostgreSQL Row-Level Security.
- **Idempotency Key**: Unique client-supplied token preventing duplicate transaction execution.
- **Outbox Pattern**: Transactional persistence of domain events before asynchronous publishing.

---

## 2. Overall Description

### 2.1 Product Perspective & Context
```
+-------------------------------------------------------------+
|                      Fidel Hub Platform                     |
|                                                             |
|  [Web Portal (Vite+React)]     [Mobile App (React Native)]  |
|               \                            /                |
|                \                          /                 |
|             [ASP.NET Core Modular Monolith API]             |
|                             |                               |
|            [PostgreSQL with RLS + Redis Cache]              |
+-------------------------------------------------------------+
```

### 2.2 User Classes and Characteristics
- **System Super Administrator**: Platform-level oversight, tenant provisioning, system telemetry.
- **School Principal / Director**: Tenant-wide policy, academic approval, financial visibility.
- **Branch Administrator**: Day-to-day campus management, staff assignment, student admissions.
- **Teacher**: Attendance recording, gradebook entry, lesson plans, parent communications.
- **Cashier / Finance Officer**: Fee collection, invoice generation, bank reconciliation.
- **Parent / Guardian**: Fee payments, student attendance & grades viewing, notices.
- **Student**: Schedule, report cards, assignment submissions.

### 2.3 Operating Environment
- **Server**: Linux / Docker containerized ASP.NET Core runtime, PostgreSQL 16+.
- **Client Web**: Modern evergreen browsers (Chrome, Firefox, Safari, Edge).
- **Client Mobile**: Android 10+ and iOS 15+ devices with offline SQLite support.

---

## 3. Functional Requirements

### 3.1 Module 01: Tenant & Branch Management
- **FR-TEN-001**: Tenant onboarding with unique subdomain and organization metadata.
- **FR-TEN-002**: Multi-branch support with branch-scoped data isolation and unified reporting.
- **FR-TEN-003**: Subscription plan enforcement and feature gating.

### 3.2 Module 02: Identity & Access Control
- **FR-IDN-001**: Role-Based and Permission-Based Access Control (RBAC/PBAC).
- **FR-IDN-002**: Multi-tenant authentication with JWT tokens containing tenant and branch claims.
- **FR-IDN-003**: Audit logging of all administrative and authentication events.

### 3.3 Module 03: Student Information System (SIS)
- **FR-STU-001**: Student profile management with national ID, birth certificate, and medical records.
- **FR-STU-002**: Guardian relationship mapping (primary guardian, emergency contact, billing party).
- **FR-STU-003**: Academic enrollment, transfer between branches, and alumni archiving.

### 3.4 Module 04: Academic Setup & Curriculum
- **FR-ACA-001**: Dual calendar support (Ethiopian EC calendar and Gregorian GC calendar).
- **FR-ACA-002**: Multi-term structuring (Semesters, Quarters) with custom subject weighting.
- **FR-ACA-003**: Class section allocation, subject assignment, and timetable generation.

### 3.5 Module 05: Attendance Tracking
- **FR-ATT-001**: Daily homeroom and period-by-period attendance recording.
- **FR-ATT-002**: Offline attendance caching on mobile with asynchronous push synchronization.
- **FR-ATT-003**: Automated instant SMS trigger to parents upon unmarked student absence.

### 3.6 Module 06: Assessment & Examinations
- **FR-EXM-001**: Continuous assessment (tests, assignments, projects) and term exam configuration.
- **FR-EXM-002**: Grade scale definitions (Letter, Percentage, GPA, Ministry ranking).
- **FR-EXM-003**: Bulk grade entry and automated report card generation (PDF / Ethiopic script).

### 3.7 Module 07: Finance & Double-Entry Ledger
- **FR-FIN-001**: Automated term fee invoicing with optional sibling/scholarship discounts.
- **FR-FIN-002**: Double-entry ledger journal vouchers (Debit Cash/Bank, Credit Receivables/Income).
- **FR-FIN-003**: Idempotent receipt creation with printable PDF and SMS confirmation.
- **FR-FIN-004**: Telebirr, CBE Birr, and Bank slip attachment reconciliation.

---

## 4. Non-Functional Requirements

### 4.1 Security Requirements
- **NFR-SEC-001**: Strict tenant isolation enforced at database engine via PostgreSQL RLS.
- **NFR-SEC-002**: TLS 1.3 encryption in transit; AES-256 encryption at rest for sensitive data.
- **NFR-SEC-003**: Immutable audit trails for all financial entries and grade modifications.

### 4.2 Reliability & Availability
- **NFR-REL-001**: 99.9% uptime SLA for core API during school operating hours.
- **NFR-REL-002**: Zero data loss for committed financial transactions through ACID guarantees.

### 4.3 Performance Requirements
- **NFR-PRF-001**: 95th percentile API response time under 250ms for standard read operations.
- **NFR-PRF-002**: Report card batch compilation completed in under 60 seconds per 500 students.

### 4.4 Offline Capability
- **NFR-OFL-001**: Mobile application must maintain complete attendance taking offline for up to 7 days.
- **NFR-OFL-002**: Conflict resolution policy must guarantee server authority with client audit trail.

---

## 5. Verification & Acceptance Criteria
- [ ] Automated integration test verification of all tenant isolation boundaries.
- [ ] Financial double-entry audit balance verification ($\sum \text{Debit} = \sum \text{Credit}$).
- [ ] Offline simulation test for 1,000 attendance records synced concurrently.
