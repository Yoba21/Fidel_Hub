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

It will support independent school branches, Ethiopian and Gregorian calendars, and offline attendance operations.

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
10. Support offline attendance operations.
11. Enforce strict tenant and branch data isolation.

3.2 Success Criteria

The product will be considered successful when:

- A school can configure its institution and branches.
- Authorized users can manage students and guardians.
- Admission and enrollment workflows operate correctly.
- Academic structures can be configured for different schools.
- Teachers can access their assigned academic responsibilities.
- Attendance can be recorded online and offline.
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
[01/11/2026 12:33 PM] Yo: - Manage tenants.
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
[01/11/2026 12:33 PM] Yo: - Register students.
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

5.10 Offline Attendance

Purpose: Allow authorized staff to record attendance without internet connectivity.

Functional requirements:

- Download authorized attendance data.
- Store attendance data locally.
- Record attendance offline.
- Queue unsynchronized operations.
- Synchronize when connectivity returns.
- Prevent duplicate operations.
- Validate user and branch authorization.
- Detect synchronization conflicts.
- Display synchronization results.

Offline support is limited to attendance in the initial release.

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
[01/11/2026 12:33 PM] Yo: Purpose: Provide operational and management visibility.

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

When offline, attendance is stored locally and synchronized after connectivity returns.

6.6 Examination

1. Authorized user creates an assessment.
2. Assessment configuration is established.
3. Student marks are entered.
4. Marks are validated.
5. Results are calculated.
6. Authorized staff review results.
7. Results are approved and published.

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

Initial engineering targets:
[01/11/2026 12:33 PM] Yo: - Standard API reads: 95th percentile under 500 ms.
- Standard API writes: 95th percentile under 800 ms.
- Common page data should load without unnecessary sequential API calls.
- Attendance synchronization must support batched operations.

These are initial targets, not guarantees. Production targets will be validated through load testing.

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

The initial product does not include:

- Full general ledger accounting.
- Payroll processing.
- Full HR and recruitment management.
- Library management.
- Transport fleet management.
- Hostel management.
- Learning Management System functionality.
- Online learning and video classes.
- Full inventory and procurement.
- Advanced parent-teacher communication channels.
- Offline financial transactions.
- Microservices architecture.

These features may be considered in subsequent product releases through formal scope reviews.

---

10. Initial Release Priorities

P0 — Essential Foundation

- Tenant management.
- Branch management.
- User management.
- Authentication and authorization.
- Tenant and branch isolation.
- School configuration.

P1 — Core School Operations

- Student management.
- Guardian management.
- Admission management.
- Academic management.
- Staff and teacher management.
- Attendance management.

P2 — Academic and Financial Operations

- Offline attendance.
- Examination management.
- Results management.
- Fee management.
- Invoice management.
- Payment collection.
- Financial reporting.

P3 — Platform Enhancement

- Advanced dashboards.
- Extended reporting.
- Parent portal.
- Student portal.
- Notifications.
- Additional integrations.

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
- Offline attendance can be synchronized.
- Replayed synchronization operations do not create duplicates.

Examinations

- Assessments can be configured.
- Marks are validated.
- Results can be calculated and approved.
- Historical results are preserved.
[01/11/2026 12:33 PM] Yo: Finance

- Fee structures can be configured.
- Student fees can be assigned.
- Invoices can be posted.
- Payments can be confirmed and alloca