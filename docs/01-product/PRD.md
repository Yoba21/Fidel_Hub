# PRODUCT REQUIREMENTS DOCUMENT (PRD)

## Fidel Hub — School Management SaaS Platform

Document Information| Details
Document| Fidel Hub Product Requirements Document
Version| 1.0
Status| Initial product baseline
Date| November 1, 2026
Market| Ethiopia
Target institutions| K–12
Product type| Multi-tenant SaaS

---

1. Product Overview

1.1 Product Vision

Build Fidel Hub, a modern, secure, scalable School Management SaaS platform that enables Ethiopian educational institutions to manage their administrative, academic, student, staff and financial operations through one integrated system.

The platform will replace fragmented manual processes with centralized, structured and accessible digital workflows.

It will support independent school branches, Ethiopian and Gregorian calendars, and offline CBT examination operations.

1.2 Product Mission

Provide schools with reliable technology to:

- Improve administrative efficiency.
- Maintain accurate student and academic records.
- Simplify daily school operations.
- Improve attendance monitoring.
- Manage school fee collection transparently.
- Improve communication and information accessibility.
- Support data-driven institutional decisions.

1.3 Product Positioning

The product is a multi-tenant SaaS platform serving independent educational institutions.

Each subscribed school operates within its own isolated tenant environment.

Schools with multiple branches can manage their organization centrally while maintaining independent branch administration and finances.

---

2. Target Market

2.1 Geographic Market

Initial market: Ethiopia.

The platform will accommodate Ethiopian educational practices, including:

- Ethiopian calendar support.
- Gregorian calendar support.
- Ethiopian Birr (ETB).
- Ethiopian school administration workflows.
- Local educational terminology and grading configuration.
- Local date formats.

2.2 Target Institution Types

The initial product supports:

Institution| Coverage
Kindergarten| Early childhood education
Primary school| Primary grade levels
Secondary school| Secondary grade levels

The academic structure must be configurable rather than hardcoded to one school's grade system.

2.3 Customer Structure

The platform distinguishes between:

Platform: The SaaS provider operating the entire service.

Tenant: A subscribed school organization.

Branch: An independently administered campus belonging to a tenant.

Each branch maintains its own operational and financial records.

---

3. Product Goals and Objectives

3.1 Primary Goals

1. Digitize school administrative operations.
2. Centralize student and guardian information.
3. Manage admissions and student enrollment.
4. Organize academic years, terms, classes and sections.
5. Manage teachers and staff.
6. Simplify attendance recording and monitoring.
7. Manage examinations and academic results.
8. Manage school fees, invoices and payments.
9. Provide reliable reports and dashboards.
10. Support offline Computer-Based Testing (CBT) and examination operations.
11. Enforce strict tenant and branch data isolation.

3.2 Success Criteria

The product will be considered successful when:

- A school can configure its institution and branches.
- Authorized users can manage students and guardians.
- Admission and enrollment workflows operate correctly.
- Academic structures can be configured for different schools.
- Teachers can access their assigned academic responsibilities.
- CBT assessments and examinations can be conducted and taken online and offline.
- Examination results can be recorded and published through authorized workflows.
- Schools can configure fees and collect payments.
- Financial balances remain accurate.
- Branches cannot access unauthorized branch records.
- Tenants cannot access another tenant's data.
- Administrators can retrieve reliable operational reports.

---

4. User Types and Roles

The platform supports multiple user categories.

4.1 Platform-Level Users

Platform Super Administrator

Responsible for operating the SaaS platform.

Responsibilities:
- Manage tenants.
- Manage subscriptions and platform configuration.
- Monitor platform health.
- Manage platform-wide settings.
- Suspend and reactivate tenant accounts.
- Access platform-level operational reports.

Platform administration does not imply unrestricted access to school financial or student records.

Sensitive tenant data access requires explicit authorization and audit logging.

4.2 Tenant-Level Users

Tenant Administrator

Responsible for organization-wide administration.

Responsibilities:

- Manage tenant configuration.
- Manage branches.
- Manage tenant memberships.
- Assign tenant-wide permissions.
- Monitor branch operations.
- Access authorized organization-level reports.

Branch Administrator

Responsible for the administration of an individual branch.

Responsibilities:

- Configure branch settings.
- Manage branch users.
- Manage students and staff.
- Manage academic operations.
- Manage branch fee structures.
- Monitor branch performance.

4.3 Operational Users

Principal / School Administrator

- Monitor school operations.
- Review academic performance.
- Approve authorized academic activities.
- Access school reports.

Registrar

- Manage student registration.
- Process admission applications.
- Maintain student records.
- Manage enrollment and transfers.

Academic Coordinator / HOD

- Manage academic assignments.
- Coordinate teachers.
- Monitor class and subject activities.
- Review academic performance.

Teacher

- Access assigned classes and subjects.
- Record attendance.
- Manage assessments.
- Enter student marks.
- View assigned student information.

Accountant / Cashier

- Manage fee-related operations.
- Generate invoices.
- Record and confirm payments according to permissions.
- Process authorized refunds.
- Review financial reports.

Student

The student portal provides access to authorized student information.

Parent / Guardian

The guardian portal provides access to linked student information.

Portal capabilities will be introduced according to the approved release scope.

---

5. Product Modules and Functional Scope

5.1 Platform and Tenant Management

Purpose: Manage the SaaS platform and its subscribed institutions.

Functional requirements:

- Register tenants.
- Configure tenant information.
- Manage tenant status.
- Create and manage branches.
- Manage tenant memberships.
- Configure tenant-level settings.
- Enforce tenant isolation.
- Maintain tenant-level audit records.

5.2 Branch Management

Purpose: Enable independent administration of school branches.

Functional requirements:

- Create and configure branches.
- Assign branch administrators.
- Configure academic settings.
- Configure calendar preferences.
- Configure currency.
- Configure fee categories.
- Maintain independent branch records.
- Restrict users to authorized branches.

Branches must maintain independent financial operations.

5.3 User and Access Management

Purpose: Control access to platform functionality and school data.

Functional requirements:

- User registration and account creation.
- User authentication.
- Password management.
- User activation and deactivation.
- Tenant membership management.
- Branch access management.
- Role assignment.
- Granular permission assignment.
- Access revocation.
- Audit logging.

Permissions must be checked by the backend.

5.4 School Configuration

Purpose: Configure the academic and administrative structure of each branch.

Functional requirements:

- School profile.
- School contact information.
- Academic calendar settings.
- Grade configuration.
- Classroom configuration.
- Class configuration.
- Section configuration.
- Subject configuration.
- School-specific operational settings.

5.5 Student Management

Purpose: Maintain complete student records throughout their relationship with the school.

Functional requirements:
- Register students.
- Generate student identification numbers.
- Maintain student profiles.
- Manage guardian relationships.
- Maintain student contact information.
- Upload student documents.
- Track student status.
- Maintain historical enrollment.
- Record student transfers.
- Search and filter student records.

A student identity must remain independent of an individual academic enrollment.

5.6 Admission Management

Purpose: Digitize the student admission process.

Functional requirements:

- Create admission applications.
- Capture applicant information.
- Capture guardian information.
- Upload admission documents.
- Review applications.
- Approve or reject applications.
- Record admission decisions.
- Convert approved applicants into registered students.
- Create appropriate enrollment records.

Admission records must preserve their historical status and decisions.

5.7 Academic Management

Purpose: Organize the school's academic structure.

Functional requirements:

- Configure academic years.
- Configure academic terms.
- Manage grade levels.
- Create classes.
- Create sections.
- Manage subjects.
- Assign subjects to classes.
- Assign teachers to classes and subjects.
- Enroll students.
- Manage student progression.
- Maintain academic history.

Academic records are branch-specific.

5.8 Staff and Teacher Management

Purpose: Maintain school employee and teacher information.

Functional requirements:

- Register staff.
- Maintain staff profiles.
- Record employment information.
- Assign staff to branches.
- Link staff records to user accounts.
- Assign teachers to academic responsibilities.
- Maintain staff status.
- Retrieve staff reports.

5.9 Attendance Management

Purpose: Provide reliable student attendance tracking.

Functional requirements:

- Create attendance sessions.
- Select classes and sections.
- Record student attendance.
- Support attendance statuses.
- Record attendance timestamps.
- Update attendance with authorization.
- Maintain attendance history.
- Generate attendance reports.
- Track absent and late students.

Attendance must be recorded against valid student enrollment.

5.10 Offline CBT & Examination Operations

Purpose: Enable schools to administer Computer-Based Testing (CBT) and examinations reliably during internet and electrical power outages using the Local Exam Venue Server (EVS) and local SQLite storage.

Functional requirements:

- Pre-fetch AES-256 encrypted exam packages containing questions, KaTeX formulas, media attachments, and option permutations from the central cloud API.
- Store exam bundles and candidate response sessions locally in SQLite (WAL mode).
- Administer CBT assessments across the local school LAN with 100% zero external internet connectivity.
- Continuous heartbeat autosave (sub-50ms) preserving candidate answers and exact remaining countdown time.
- Instant workstation crash recovery restoring in-progress sessions without answer loss or time skew.
- Validate candidate identity and proctor exam start authorization locally.
- Queue completed examination sessions for background synchronization upon internet restoration.
- Idempotently upload completed sessions via batch synchronization (`POST /api/v1/cbt/sessions/sync`), preventing duplicate submissions or score distortion through database unique constraints.
- Maintain tamper-evident synchronization batch logs and audit records.

Offline execution and local SQLite WAL synchronization in the initial release are focused strictly on Computer-Based Testing (CBT) and Examinations. Teacher attendance operations run online in the initial release (with mobile offline attendance synchronization deferred to a future phase).

5.11 Examination and Results Management

Purpose: Manage assessments and student academic performance.

Functional requirements:

- Configure assessments.
- Define maximum marks.
- Configure grading rules.
- Record student marks.
- Validate marks.
- Calculate results.
- Review academic performance.
- Approve and publish results.
- Generate broadsheets.
- Generate report cards.
- Maintain historical examination results.

Grading rules must be configurable to accommodate institutional requirements.

5.12 Fee Management

Purpose: Manage school fees and student payment operations.

Functional requirements:

- Configure fee categories.
- Create fee structures.
- Define fee structure lines.
- Assign fees to students.
- Generate invoices.
- Post invoices.
- Record payments.
- Confirm payments.
- Allocate payments to invoices.
- Track outstanding balances.
- Process refunds.
- Generate financial reports.

Financial Requirements

- Financial records must remain branch-specific.
- Fee assignments must preserve historical fee snapshots.
- Posted invoices must not be directly modified.
- Confirmed payments must not be duplicated.
- Payment allocations must not exceed eligible invoice balances.
- Refunds must not exceed refundable amounts.
- All financial operations must be auditable.

The initial product covers fee management and does not implement a complete general ledger.

5.13 Reporting and Dashboards
Purpose: Provide operational and management visibility.

Initial reporting scope:

Platform reports

- Tenant statistics.
- Branch statistics.
- Platform activity.

School reports

- Student statistics.
- Enrollment statistics.
- Attendance statistics.
- Staff statistics.
- Academic performance.

Financial reports

- Fee collection.
- Outstanding balances.
- Payment history.
- Refund history.
- Branch financial summaries.

Reports must respect tenant, branch and user permissions.

5.14 Audit and Activity Management

Purpose: Maintain accountability and traceability.

Functional requirements:

- Record important user actions.
- Record sensitive data changes.
- Record financial operations.
- Record authorization-related events.
- Identify the responsible user.
- Associate activities with tenants and branches.
- Preserve historical audit records.

---

6. Core Business Workflows

6.1 Tenant Registration

1. Platform administrator creates a tenant.
2. Tenant information is registered.
3. Initial tenant administrator is assigned.
4. Tenant configuration is initialized.
5. The tenant creates its first branch.
6. Branch administration is configured.
7. Authorized users receive access.

6.2 Branch Administration

1. Tenant administrator creates a branch.
2. Branch settings are configured.
3. Branch administrator is assigned.
4. Academic configuration is established.
5. Users receive branch-specific access.
6. Branch operations begin independently.

6.3 Student Admission

1. Applicant information is submitted.
2. Guardian information is recorded.
3. Required documents are collected.
4. Registrar reviews the application.
5. Application is approved or rejected.
6. Approved applicants become registered students.
7. Student enrollment is created.
8. Student becomes available in the academic system.

6.4 Academic Enrollment

1. Academic year is configured.
2. Classes and sections are established.
3. Student enrollment is created.
4. Student is assigned to the appropriate class and section.
5. Teacher assignments are configured.
6. Student becomes available for academic and attendance operations.

6.5 Attendance

1. Authorized user selects a class and section.
2. Attendance session is opened.
3. Student attendance is recorded.
4. Attendance is submitted.
5. Records are validated and stored.
6. Attendance reports are updated.

6.6 Examination & CBT Operations

1. Authorized examiner creates and schedules an assessment in the Question Bank.
2. Assessment package is compiled, signed, and encrypted for venue export.
3. Local Exam Venue Server downloads and stages the encrypted exam bundle.
4. Candidates sit for examinations on school LAN workstations (100% offline).
5. Responses and timers are continuously autosaved to local SQLite WAL storage.
6. Exam session is submitted and encrypted at the venue server upon completion.
7. Venue server idempotently synchronizes completed exam batches to the cloud API upon internet reconnection.
8. Results are verified, calculated, reviewed by authorized staff, and published.

6.7 Fee Collection

1. Branch configures fee structures.
2. Fees are assigned to students.
3. Invoices are generated.
4. Invoices are posted.
5. Payment is received.
6. Payment is confirmed.
7. Payment is allocated.
8. Outstanding balances are recalculated.
9. Financial records are updated and audited.

6.8 Student Transfer

1. Authorized user initiates transfer.
2. Current enrollment is closed through a controlled operation.
3. Historical academic records are preserved.
4. Destination branch or school enrollment is created according to authorized transfer rules.
5. Access and branch-specific records are updated.

---

7. Non-Functional Requirements

7.1 Security

- Strong authentication.
- Backend authorization.
- Role-based permissions.
- Tenant isolation.
- Branch isolation.
- PostgreSQL Row-Level Security.
- Secure password handling.
- Protected document access.
- Audit logging.
- Secure API communication.
- Least-privilege database access.

7.2 Performance

The application must provide responsive user experiences under normal operating conditions.

Performance must be measured through representative workloads.

Engineering performance benchmarks (aligned with SRS.md NFR-PRF-001 through NFR-PRF-003):
- Standard API reads and directory searches: 95th percentile under 200 ms.
- Complex analytical reads and report compilations: 95th percentile under 500 ms.
- Standard transactional API writes: 95th percentile under 800 ms.
- CBT local autosave latency: sub-50ms per heartbeat autosave transaction via SQLite WAL mode.
- Report card batch generation: compilation of 1,000 dual-calendar PDF report cards within 90 seconds via background worker queues.
- Common page data must load within a single composite query without sequential N+1 API calls.
- Offline CBT examination synchronization must support batched, idempotent operations with zero duplicate records.

These benchmarks are verified continuously through automated k6 load tests and CI/CD performance gates.

7.3 Availability and Reliability

- Reliable database transactions.
- Automated database backups.
- Recovery procedures.
- Application health checks.
- Background job monitoring.
- Idempotent synchronization.
- Controlled deployment and rollback procedures.

7.4 Scalability

The architecture must support increasing numbers of:

- Tenants.
- Branches.
- Students.
- Staff.
- Attendance records.
- Financial transactions.
- Concurrent users.

The application begins as a modular monolith.

7.5 Maintainability

- Consistent coding standards.
- Modular business organization.
- Automated testing.
- Version-controlled migrations.
- Documented APIs.
- Clear business rules.
- Structured logging.
- Technical documentation.

7.6 Localization

The product must support:

- Ethiopian calendar.
- Gregorian calendar.
- ETB currency.
- Configurable date formats.
- Configurable academic years and terms.
- Localization-ready user interfaces.

---

8. Data Ownership and Isolation

1. Every tenant owns its school organization data.
2. Every branch owns its operational records.
3. Student identities are maintained independently of enrollments.
4. Historical academic records are preserved.
5. Branch financial operations remain independent.
6. Tenant data is isolated through PostgreSQL RLS and application authorization.
7. Branch access is explicitly granted.
8. Audit records preserve important historical actions.

---

9. Product Scope Boundaries

### 9.1 Excluded / Out-of-Scope Capabilities
The Fidel Hub platform explicitly excludes the following capabilities from its architecture baseline:
- Full double-entry general ledger accounting (chart of accounts, trial balances, journal ledgers)—the system strictly focuses on Student Fee Management and Invoicing.
- Payroll processing and employee tax deductions.
- Full human resources (HR), recruitment, and staff performance appraisals.
- Transport fleet GPS tracking and vehicle maintenance.
- Hostel and boarding dormitory room allocations.
- Learning Management System (LMS) features such as real-time video conferencing, SCORM packages, and live screen sharing.
- Enterprise-wide warehouse inventory and procurement workflows.
- Offline financial and invoice payment processing (all fee transactions require online connection).
- Distributed microservices architecture (the application strictly operates as a modular monolith).

### 9.2 Phased Expanded Institutional Modules (Phase 2 / P2 Scope)
The following 5 modules are fully incorporated in the system architecture, database schema, and project solution layout, and are scheduled for delivery in Phase 2 (P2) of the product roadmap:
- **Module 11: Library & Media Resource Management** (`FidelHub.Modules.Library`, `library` schema): Physical book cataloging, barcode circulation, digital e-book repository, and automated overdue fee calculation.
- **Module 12: Laboratory & Specialized ICT Facilities** (`FidelHub.Modules.Facilities`, `facilities` schema): Science apparatus and chemical inventory management, hazard tracking, and lab room reservation scheduling.
- **Module 13: Student Health & Clinic Dispensary** (`FidelHub.Modules.Health`, `health` schema): Confidential student medical profiles, clinic dispensary visits, vital signs logging, and urgent guardian emergency SMS alerts.
- **Module 14: Staff/Teacher Grade Modification Audit** (`FidelHub.Modules.Audit`, `audit` schema): Tamper-evident immutable audit trails tracking all post-deadline mark modifications with mandatory reason codes.
- **Module 15: Platform SuperAdmin CRM & Subscription Quotas** (`FidelHub.Modules.Platform`, `platform` schema): Multi-tenant subscription tier enforcement, active student headcount quotas, and automated tenant snapshot exports.

---

10. Initial Release Priorities

### Phase 1: Core Foundation & Multi-Tenancy Kernel (P0)
- Organization, Tenant, and Branch campus management (`FidelHub.Modules.Tenants`).
- Identity & Access Management with Better Auth service integration (`FidelHub.Modules.Identity`).
- PostgreSQL Row-Level Security (RLS) enforcement across 15 schemas.
- Dual Ethiopian/Gregorian calendar engine and school configuration (`FidelHub.Modules.Academics`).

### Phase 1: Core School Operations & Student SIS (P1)
- Student Information System (SIS) with three-part Ethiopian naming and guardian links (`FidelHub.Modules.Students`).
- Admissions processing and online application intake (`FidelHub.Modules.Admissions`).
- Timetables, classroom schedules, subjects, and teacher lesson plans (`FidelHub.Modules.Academics`).
- Teacher management and class assignments.
- Homeroom and period-by-period daily attendance tracking (`FidelHub.Modules.Attendance`).

### Phase 1: CBT, Grading & Student Fee Integrity (P2 Core)
- Question Bank authoring, assessment assembly, and KaTeX formula support (`FidelHub.Modules.Examinations`).
- Local Exam Venue Server (EVS) with 100% offline LAN test delivery and SQLite WAL autosave.
- Continuous Assessment (CA) gradebook with spreadsheet-style entry and approval locks (`FidelHub.Modules.Grading`).
- Student Fee Management, snapshot agreements, batch invoicing, and idempotent payments (`FidelHub.Modules.Finance`).
- Communication & automated unexcused absence SMS dispatch (`FidelHub.Modules.Communication`).
- Reporting, transcript compilation, and Ge'ez PDF report cards (`FidelHub.Modules.Reporting`).

### Phase 2: Expanded Institutional Modules (P2 Expanded)
- Library & Media Resource Management (`FidelHub.Modules.Library`).
- Laboratory & Specialized ICT Facility Management (`FidelHub.Modules.Facilities`).
- Student Health, Clinic Dispensary & Medical Care (`FidelHub.Modules.Health`).
- Staff/Teacher Grade Modification & Security Audit Trail (`FidelHub.Modules.Audit`).
- Platform SuperAdmin Management & Subscription Quotas (`FidelHub.Modules.Platform`).

---

11. Product Acceptance Criteria

The initial product is accepted when the following conditions are demonstrated:

Platform

- Tenants can be created and managed.
- Branches can be independently configured.
- Users can be assigned to authorized tenants and branches.

Security

- Cross-tenant access is denied.
- Unauthorized branch access is denied.
- Revoked memberships cannot access protected data.
- Sensitive actions are audited.

Students and Academics

- Students can be registered.
- Guardians can be linked to students.
- Admission applications can be processed.
- Students can be enrolled in valid classes and sections.
- Academic history is preserved.

Attendance

- Authorized users can record attendance.
- Attendance is linked to valid enrollment.

Examinations & Offline CBT

- Assessments and Question Bank papers can be configured with KaTeX formulas.
- Offline CBT examination packages can be exported, administered locally on LAN, and synchronized idempotently.
- Replayed synchronization operations do not create duplicates.
- Marks are validated.
- Results can be calculated and approved.
- Historical results are preserved.

Finance

- Fee structures can be configured.
- Student fees can be assigned.
- Invoices can be posted.
- Payments can be confirmed and allocated against posted invoices without over-allocation.
- Official sequential receipts are generated with tamper-evident verification codes.
- Credit balances, discounts, and partial payments are tracked accurately.
- Financial transactions maintain an immutable audit trail without full double-entry general ledger complexity.

Communication & Notifications

- School-wide and branch-specific announcements can be composed and published to targeted audiences.
- Automated unexcused absence SMS alerts are triggered upon homeroom attendance finalization and dispatched within 15 minutes.
- Guardian communication preferences (SMS, email, push) are strictly respected.
- Notification dispatch history and delivery receipts are permanently logged and auditable.

Reporting, Transcripts & Analytics

- Academic report cards and student transcripts compile accurately with dual Ethiopian/Gregorian dates, Ge'ez Amharic typography, and verification QR codes.
- Real-time operational dashboards reflect live branch metrics for admissions, enrollment, daily attendance, and fee collection.
- Bulk report generation (e.g., 1,000 PDF report cards) compiles within 90 seconds without web server timeouts.
- All analytical exports (PDF, Excel, CSV) execute asynchronously with downloadable artifact links.

Phase 2 Expanded Institutional Modules (Operational Criteria)

- Library & Media: Physical book cataloging, barcode circulation, digital e-book repository, and automated overdue fine calculations operate reliably.
- Facilities & Lab Safety: Science and ICT laboratory apparatus inventories, chemical hazard ratings, and room reservation scheduling prevent timetable collisions.
- Student Health & Clinic: Student medical profiles (allergies, chronic conditions), clinic dispensary visit logging, and urgent guardian SMS notifications function seamlessly.
- Grade Modification Audit: Tamper-evident audit logging records every post-deadline mark adjustment with mandatory reason codes, previous scores, and approver identity.
- Platform SuperAdmin & Subscriptions: SaaS tenant subscription tier limits (student count ceilings, branch limits, document storage quotas) are enforced automatically.

---

12. Document Sign-off & Revision Approvals

| Role | Organization / Working Group | Representative | Status | Sign-off Date |
| :--- | :--- | :--- | :--- | :--- |
| **Product Manager** | Fidel Hub Product Working Group | Product Directorate | Approved Baseline | 2026-11-01 |
| **Lead Technical Architect** | Architecture Review Board | Technical Architecture Directorate | Approved Baseline | 2026-11-01 |
| **Backend Engineering Lead** | Core Platform Engineering | Backend Directorate | Approved Baseline | 2026-11-01 |
| **Frontend & Mobile Lead** | Client Experience Working Group | Client Engineering Directorate | Approved Baseline | 2026-11-01 |
| **Quality Assurance Lead** | Verification & Compliance | QA Directorate | Approved Baseline | 2026-11-01 |