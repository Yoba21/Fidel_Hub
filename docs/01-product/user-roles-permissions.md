# USER ROLES & PERMISSIONS MATRIX (URP)

## Fidel Hub — Access Control & Authorization Framework

| Document Information | Details |
| :--- | :--- |
| **Document ID** | URP-001 |
| **Version** | 1.1 (Expanded Multi-Role Institutional Matrix) |
| **Status** | Approved Baseline |
| **Authentication Engine** | Better Auth (Node.js/PostgreSQL) with Organizations & JWT Plugins |
| **Authorization Engine** | ASP.NET Core Policy-Based Authorization & PostgreSQL Kernel RLS |

---

## 1. Authorization Architecture & Multi-Tenancy Scoping

Fidel Hub employs a **hybrid Role-Based and Permission-Based Access Control (RBAC/PBAC)** model enforced across three strict dimensions:

```
+-----------------------------------------------------------------------------------+
| 1. TENANT SCOPE (School Organization)                                              |
|    - Verified via JWT claim 'tenant_id'.                                          |
|    - Isolated at the database kernel via PostgreSQL Row-Level Security (RLS).     |
+-----------------------------------------------------------------------------------+
                                         |
                                         v
+-----------------------------------------------------------------------------------+
| 2. BRANCH SCOPE (Campus / Location)                                                |
|    - Verified via JWT claim 'branch_id' against 'identity.user_branch_access'.     |
|    - Tenant-wide administrators have universal branch access within their tenant. |
|    - Branch staff are hard-locked to their assigned physical campus.              |
+-----------------------------------------------------------------------------------+
                                         |
                                         v
+-----------------------------------------------------------------------------------+
| 3. GRANULAR CAPABILITY PERMISSIONS (Action / Module)                              |
|    - Verified via ASP.NET Core Policy Handlers: [Authorize(Policy = "...")]       |
|    - Derived from the user's role assignments stored in 'identity.user_roles'.    |
+-----------------------------------------------------------------------------------+
```

---

## 2. Standard Institutional Roles

| Role Code | Role Title | Ownership Scope | Primary Responsibilities |
| :--- | :--- | :--- | :--- |
| `PLATFORM_SUPERADMIN` | Platform Super Administrator | Global Platform | SaaS organization provisioning, system telemetry, subscription quotas, tenant backup exports. |
| `TENANT_PRINCIPAL` | School Principal / Director | Tenant-Wide | Organization policy, cross-branch analytics, academic lock approval, post-deadline grade adjustments. |
| `BRANCH_ADMIN` | Branch Campus Administrator | Branch-Scoped | Daily campus operations, staff room allocations, local policy enforcement, emergency response. |
| `HEAD_OF_DEPARTMENT` | Head of Department (HOD) | Branch & Dept | Curriculum review, lesson plan approval, Question Bank quality assurance, subject teacher oversight. |
| `TEACHER` | Classroom Teacher / Instructor | Branch & Class | Daily attendance, assignment scoring, Continuous Assessment (CA) entry, lesson plan authoring. |
| `EXAM_COORDINATOR` | Examination & CBT Coordinator| Branch-Scoped | Question bank management, exam paper assembly, CBT venue server unlocking, psychometrics. |
| `CASHIER` | School Cashier / Finance Officer | Branch-Scoped | Fee structure setups, invoice generation, payment receipting, bank slips (*no GL accounting*). |
| `REGISTRAR` | Registrar & Admissions Officer | Branch-Scoped | Student registration, guardian mapping, health notes, branch transfers, student files. |
| `LIBRARIAN` | School Librarian / Media Specialist | Branch-Scoped | Book cataloging, circulation desk checkout/returns, overdue fine assessments, digital media assets. |
| `LAB_TECHNICIAN` | Laboratory & ICT Facility Tech | Branch-Scoped | Apparatus registry, chemical reagents inventory, safety checklist, lab room booking schedule. |
| `SCHOOL_NURSE` | School Health Officer / Nurse | Branch-Scoped | Student medical profiles, daily clinic dispensary visits, vital signs, rest-bed tracking, parent SMS. |
| `PARENT` | Parent / Guardian | Household | View child attendance, grades, report cards, fee statements, make payments, receive clinic alerts. |
| `STUDENT` | Enrolled Student / Candidate | Personal Scope | View schedules, homework, take CBT assessments, view academic results, access digital library. |

---

## 3. Granular Permissions Catalog

### 3.1 Organization & Branches (`org:*`)
- `org.tenant.view`: View school organization profile and settings.
- `org.tenant.edit`: Update school branding, default currency, and contacts.
- `org.branch.create`: Provision a new campus or physical branch.
- `org.branch.edit`: Update branch details, addresses, and academic calendar settings.
- `org.calendar.manage`: Configure Ethiopian (EC) and Gregorian (GC) academic terms.

### 3.2 Student Information System & Admissions (`sis:*`)
- `sis.student.view`: View student directory and student profile records.
- `sis.student.create`: Register a newly admitted student.
- `sis.student.edit`: Update biodata, medical notes, and family relationships.
- `sis.student.transfer`: Initiate and approve branch-to-branch student transfers.
- `sis.student.archive`: Archive, graduate, or withdraw student records.
- `sis.guardian.manage`: Map guardians, emergency contacts, and billing recipients.
- `sis.admission.review`: Process admission applications, review documents, approve/reject.

### 3.3 Academics, Curriculum & Timetables (`acad:*`)
- `acad.class.manage`: Setup grade levels, class sections, and classroom seat capacities.
- `acad.subject.manage`: Define subject courses, credit hours, and departments.
- `acad.teacher.assign`: Allocate teachers to specific subject-class-section tuples.
- `acad.timetable.edit`: Build and modify classroom timetable schedule grids.
- `acad.lessons.submit`: Author and submit weekly lesson plans for HOD review.
- `acad.lessons.approve`: Review, comment, and officially approve teacher lesson plans.
- `acad.traits.evaluate`: Enter affective and psychomotor behavioral trait ratings.
- `acad.nursery.log`: Record daily feeding, nap, and milestone activities for Kindergarten/Nursery.

### 3.4 Attendance (`att:*`)
- `att.session.take`: Record daily morning homeroom and period-by-period attendance.
- `att.session.correct`: Edit historical attendance records with mandatory audit justification.
- `att.reports.view`: View truancy logs, student attendance rates, and absence stats.

### 3.5 Question Bank & Examination Management (`exam:*`)
- `exam.question.create`: Author questions (MCQ, True/False, Fill-Blank, Theory) with KaTeX math.
- `exam.question.review`: Review, approve, and tag questions in the central Question Bank.
- `exam.question.import`: Execute bulk question bank import via ZIP / CSV bundles.
- `exam.paper.create`: Assemble exam papers (randomized selection or fixed blueprints).
- `exam.paper.schedule`: Schedule assessment dates, time windows, and venue assignments.
- `exam.cbt.package.export`: Export AES-256 encrypted exam packages for offline venue servers.
- `exam.cbt.package.unlock`: Unlock time-locked exam packages at official start schedule.
- `exam.theory.grade`: Score subjective theory questions using structured marking rubrics.
- `exam.results.publish`: Finalize and release official examination scores to parents/students.

### 3.6 Continuous Assessment, Gradebook & Report Cards (`grade:*`)
- `grade.ca.configure`: Setup term assessment weighting components (e.g. 15% Quiz, 35% Midterm).
- `grade.marksheet.enter`: Input and edit student test scores in the spreadsheet gradebook.
- `grade.marksheet.lock`: Lock gradebook marksheet, preventing further teacher alterations.
- `grade.reports.generate`: Batch generate dual-script PDF report cards with QR verification.

### 3.7 Fee Management & Invoicing (`fee:*`)
- `fee.structure.manage`: Setup fee categories, branch fee structures, and installment policies.
- `fee.snapshot.assign`: Assign fee structures to students, preserving historical snapshots.
- `fee.invoice.generate`: Batch generate term fee invoices for enrolled students.
- `fee.invoice.post`: Finalize and post invoices, locking them from in-place updates.
- `fee.payment.record`: Receive payments (Cash, Telebirr, CBE, Bank Slip) with idempotency tokens.
- `fee.receipt.print`: Issue and print official sequential payment receipts.
- `fee.statement.view`: Generate comprehensive student fee statements and outstanding balances.

### 3.8 Communications & System Alerts (`comms:*`)
- `comms.broadcast.send`: Dispatch school-wide or branch-wide SMS and email announcements.
- `comms.alerts.manage`: Configure automated trigger rules (instant unexcused absence SMS).

### 3.9 Library & Digital Media Management (`lib:*`)
- `lib.catalog.manage`: Add, catalog, and update physical books and digital learning documents.
- `lib.loan.checkout`: Issue book loans to students and staff with due dates.
- `lib.loan.return`: Process book returns, inspect physical condition, and log damage/loss.
- `lib.fine.assess`: Assess overdue fines directly into the student fee ledger.
- `lib.fine.waive`: Waive or discount overdue library fines (Requires Branch Admin approval).

### 3.10 Laboratory & Specialized Facilities (`lab:*`)
- `lab.equipment.manage`: Register, inspect, and update science apparatus and ICT lab hardware.
- `lab.reagents.track`: Manage chemical inventories, safety limits, and expiration dates.
- `lab.session.book`: Reserve laboratory rooms and equipment without schedule collision.
- `lab.incident.log`: Record equipment breakage, safety violations, or chemical accidents.

### 3.11 Student Health & Clinic Dispensary (`health:*`)
- `health.profile.view`: View student health profile, chronic conditions, and emergency contacts.
- `health.profile.edit`: Record allergies, medical warnings, and blood group.
- `health.clinic.record`: Log dispensary visits, vitals, presenting symptoms, and treatments.
- `health.clinic.escalate`: Authorize rest bed admission and trigger urgent SMS alerts to parents.

### 3.12 Staff/Teacher Grade Access Audit (`audit:*`)
- `audit.grade.view`: View comprehensive grade viewing, entry, edit, and export audit trails.
- `audit.grade.override`: Authorize out-of-schedule grade modifications after term lock date.

### 3.13 Platform SuperAdmin & Subscriptions (`platform:*`)
- `platform.tenant.provision`: Onboard, activate, or suspend school tenant subscriptions.
- `platform.quota.configure`: Set maximum student enrollment limits and branch allocations.
- `platform.backup.export`: Generate and download encrypted tenant database archives.

---

## 4. Comprehensive Role-Permission Matrix

| Permission Code | Super Admin | School Principal | Branch Admin | Head of Dept | Teacher | Exam Coord | Cashier | Registrar | Librarian | Lab Tech | Nurse | Parent | Student |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **Organization & Branches** | | | | | | | | | | | | | |
| `org.tenant.manage` | **Full** | View/Edit | None | None | None | None | None | None | None | None | None | None | None |
| `org.branch.manage` | **Full** | **Full** | Assigned | None | None | None | None | None | None | None | None | None | None |
| `org.calendar.manage` | **Full** | **Full** | Assigned | View | View | View | View | View | View | View | View | View | View |
| **Student SIS & Admissions** | | | | | | | | | | | | | |
| `sis.student.view` | All | All | Assigned | Assigned | Assigned | Assigned | Assigned | **Full** | Assigned | Assigned | Assigned | Own Child | Self |
| `sis.student.create` | Yes | Yes | Yes | No | No | No | No | **Full** | No | No | No | No | No |
| `sis.student.transfer`| Yes | **Full** | Request | No | No | No | No | Execute | No | No | No | No | No |
| `sis.admission.review`| Yes | **Full** | **Full** | No | No | No | No | **Full** | No | No | No | Apply | No |
| **Academics & Curriculum** | | | | | | | | | | | | | |
| `acad.class.manage` | Yes | **Full** | **Full** | View | View | View | View | View | View | View | View | View | View |
| `acad.subject.manage` | Yes | **Full** | **Full** | **Full** | View | View | No | View | View | View | View | View | View |
| `acad.teacher.assign` | Yes | **Full** | **Full** | Suggest | No | No | No | No | No | No | No | No | No |
| `acad.timetable.edit` | Yes | **Full** | **Full** | Dept View | View | View | No | No | View | View | View | View | View |
| `acad.lessons.submit` | No | View | View | Review | **Full** | No | No | No | No | No | No | No | No |
| `acad.lessons.approve`| No | **Full** | **Full** | **Full** | No | No | No | No | No | No | No | No | No |
| `acad.traits.evaluate`| No | View | Assigned | Assigned | **Full** | No | No | No | No | No | No | View | View |
| `acad.nursery.log` | No | View | Assigned | Assigned | **Full** | No | No | Assist | No | No | Assist | View Child| No |
| **Attendance Tracking** | | | | | | | | | | | | | |
| `att.session.take` | No | View | Assigned | Assigned | **Assigned**| No | No | No | No | No | No | No | No |
| `att.session.correct` | Yes | **Full** | **Full** | Dept Only | Request | No | No | No | No | No | No | No | No |
| `att.reports.view` | All | All | Assigned | Assigned | Assigned | View | View | View | View | View | View | Own Child | Self |
| **Question Bank & CBT Exams** | | | | | | | | | | | | | |
| `exam.question.create`| No | View | View | **Full** | **Full** | **Full** | No | No | No | No | No | No | No |
| `exam.question.review`| No | View | View | **Full** | No | **Full** | No | No | No | No | No | No | No |
| `exam.paper.create` | No | View | View | **Full** | Assist | **Full** | No | No | No | No | No | No | No |
| `exam.cbt.package` | No | View | View | No | No | **Full** | No | No | No | No | No | No | Sit Only |
| `exam.theory.grade` | No | View | View | **Full** | **Full** | Assist | No | No | No | No | No | No | No |
| `exam.results.publish`| Yes | **Full** | **Full** | Review | No | Execute | No | No | No | No | No | View | View |
| **Continuous Assessment & Gradebook** | | | | | | | | | | | | | |
| `grade.ca.configure` | Yes | **Full** | **Full** | **Full** | No | Assist | No | No | No | No | No | No | No |
| `grade.marksheet.enter`| No | View | View | Assigned | **Assigned**| View | No | No | No | No | No | No | No |
| `grade.marksheet.lock` | Yes | **Full** | **Full** | **Full** | No | No | No | No | No | No | No | No | No |
| `grade.reports.generate`| Yes | **Full** | **Full** | Assigned | Assigned | Assigned | View | **Full** | No | No | No | Own Child | Self |
| **Fee Management & Invoicing** | | | | | | | | | | | | | |
| `fee.structure.manage`| Yes | **Full** | **Full** | No | No | No | **Full** | No | No | No | No | No | No |
| `fee.invoice.generate`| Yes | **Full** | **Full** | No | No | No | **Full** | No | No | No | No | No | No |
| `fee.payment.record` | No | View | View | No | No | No | **Full** | No | No | No | No | Pay Online| View |
| `fee.receipt.print` | No | View | View | No | No | No | **Full** | No | No | No | No | Download | View |
| `fee.statement.view` | All | All | Assigned | No | No | No | **Full** | Assigned | View | No | No | Own Child | Self |
| **Communications & Alerts** | | | | | | | | | | | | | |
| `comms.broadcast.send`| Yes | **Full** | **Full** | Dept Only | Class Only| No | No | No | No | No | No | No | No |
| `comms.alerts.manage` | Yes | **Full** | **Full** | No | No | No | No | No | No | No | No | No | No |
| **Library & Media Management** | | | | | | | | | | | | | |
| `lib.catalog.manage` | Yes | View | Assigned | Suggest | Suggest | No | No | No | **Full** | No | No | View | View |
| `lib.loan.checkout` | No | View | Assigned | No | No | No | No | No | **Full** | No | No | No | Request |
| `lib.loan.return` | No | View | Assigned | No | No | No | No | No | **Full** | No | No | No | No |
| `lib.fine.assess` | No | View | Assigned | No | No | No | Assist | No | **Full** | No | No | View | View |
| `lib.fine.waive` | Yes | **Full** | **Full** | No | No | No | Assist | No | Request | No | No | No | No |
| **Laboratory & Facilities** | | | | | | | | | | | | | |
| `lab.equipment.manage`| Yes | View | Assigned | Dept View | View | No | No | No | No | **Full** | No | No | No |
| `lab.reagents.track` | Yes | View | Assigned | Dept View | View | No | No | No | No | **Full** | No | No | No |
| `lab.session.book` | No | View | Assigned | Review | **Book** | No | No | No | No | **Confirm**| No | No | No |
| `lab.incident.log` | Yes | **Full** | **Full** | Dept Only | Submit | No | No | No | No | **Full** | Assist | View Child| No |
| **Student Health & Clinic** | | | | | | | | | | | | | |
| `health.profile.view`| Yes | All | Assigned | No | Allergies | No | No | View | No | No | **Full** | Own Child | Self |
| `health.profile.edit`| No | View | Assigned | No | No | No | No | Assist | No | No | **Full** | Submit | No |
| `health.clinic.record`| No | View | Assigned | No | No | No | No | No | No | No | **Full** | View Child| View |
| `health.clinic.escalate`| Yes| **Full**| **Full** | No | No | No | No | No | No | No | **Full** | Alerted | Alerted |
| **Staff/Teacher Audit Trail** | | | | | | | | | | | | | |
| `audit.grade.view` | Yes | **Full** | **Full** | Dept Only | Own Only | View | No | No | No | No | No | No | No |
| `audit.grade.override`| No | **Full** | Request | No | No | No | No | No | No | No | No | No | No |
| **Platform Administration** | | | | | | | | | | | | | |
| `platform.tenant.manage`| **Full**| No | No | No | No | No | No | No | No | No | No | No | No |
| `platform.quota.manage`| **Full**| View | No | No | No | No | No | No | No | No | No | No | No |
| `platform.backup.export`| **Full**| **Full** | Request | No | No | No | No | No | No | No | No | No | No |

---

## 5. Security & Enforcement Mechanisms

1. **Better Auth JWT Claims Contract**:
   ```json
   {
     "sub": "4a73e86c-7f52-4467-9321-789a421b8c10",
     "email": "examiner.bekele@fidelhub.com",
     "tenant_id": "11111111-1111-1111-1111-111111111111",
     "branch_id": "22222222-2222-2222-2222-222222222222",
     "roles": ["EXAM_COORDINATOR", "TEACHER"],
     "permissions": [
       "exam.question.create",
       "exam.cbt.package.export",
       "att.session.take",
       "grade.marksheet.enter",
       "lib.loan.checkout"
     ]
   }
   ```
2. **ASP.NET Core Authorization Handlers**:
   - Every request passes through `TenantBranchAuthorizationMiddleware`.
   - Actions decorated with `[RequirePermission("exam.cbt.package.export")]`.
   - The handler confirms:
     1. Token signature is valid from Better Auth public JWKS.
     2. Target resource's `tenant_id` matches token `tenant_id`.
     3. Target resource's `branch_id` matches token `branch_id` (or user possesses tenant-wide authority).
3. **Database Kernel RLS Guarantee**:
   - Connection executes: `SET LOCAL app.current_tenant_id = '...'`.
   - Even if an application controller failed to perform a manual WHERE clause check, PostgreSQL rejects cross-tenant reads or writes with zero rows returned.
