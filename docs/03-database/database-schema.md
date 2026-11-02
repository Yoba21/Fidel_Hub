# DATABASE DESIGN SPECIFICATION (DDS)

## Fidel Hub — School Management SaaS Platform

| Document Information | Details |
| :--- | :--- |
| **Document ID** | DDS-004 |
| **Version** | 1.0 |
| **Status** | Database Design Baseline |
| **Date** | 1 November 2026 |

---

## 1. Introduction

### 1.1 Purpose

This document defines the database architecture and logical database design for the Fidel Hub School Management SaaS Platform.

It provides the database implementation reference for:

- Backend developers.
- Database engineers.
- Technical leads.
- QA engineers.
- AI coding agents.

It defines the database schemas, entities, relationships, constraints, indexes, tenant isolation, branch isolation, and migration strategy.

### 1.2 Database Technology

| Component | Technology |
| :--- | :--- |
| **Database** | PostgreSQL |
| **ORM** | Entity Framework Core 10 (.NET 8 LTS) |
| **Database provider** | Npgsql |
| **Primary key strategy** | UUID |
| **Monetary values** | NUMERIC(14,2) |
| **Date storage** | Gregorian dates |
| **Tenant isolation** | PostgreSQL RLS |
| **Branch isolation** | PostgreSQL RLS and application authorization |
| **Schema management** | EF Core migrations and reviewed SQL scripts |

### 1.3 Database Architecture

The platform will use:

One PostgreSQL database, shared tables, and logical separation through PostgreSQL schemas.

Each tenant's data is stored in the same database as other tenants' data, but every tenant-owned record must be protected by tenant ownership rules.

Branch-owned records must additionally be restricted to authorized branches.

The database must not rely on frontend filtering for security.

---

## 2. Core Database Design Principles

1. Use UUID primary keys for application entities.
2. Every tenant-owned record must contain `tenant_id`.
3. Every branch-owned record must contain both `tenant_id` and `branch_id`.
4. Use foreign keys to enforce relational integrity.
5. Use composite foreign keys where necessary to prevent cross-tenant and cross-branch relationships.
6. Use PostgreSQL RLS for tenant and branch isolation.
7. Use database transactions for operations involving multiple related records.
8. Use `NUMERIC(14,2)` for monetary amounts.
9. Store authoritative dates in Gregorian format.
10. Store timestamps in UTC.
11. Use explicit status fields and controlled state transitions.
12. Preserve important academic and financial history.
13. Avoid hard deletion of important business records.
14. Create indexes based on actual query patterns.
15. Keep database migrations version-controlled.

---

## 3. PostgreSQL Schema Organization

The database will use the following logical schemas.

| Schema | Responsibility |
| :--- | :--- |
| `platform` | SaaS platform and tenant administration |
| `identity` | Users, roles, permissions, and access |
| `school` | Physical branch campuses, academic calendars, branch settings |
| `students` | Student demographic master records, guardian mappings |
| `admissions` | Prospective student applications and intake screening |
| `academic` | Class levels, streams, subjects, timetables, lesson plans |
| `staff` | Staff and teacher records |
| `attendance` | Attendance sessions and records |
| `examinations` | Assessments, marks, and offline CBT sessions |
| `finance` | Fees, invoices, payments, and refunds |
| `library` | Book cataloging, circulation loans, overdue fines |
| `facilities` | Lab equipment registry, room booking, incident logs |
| `health` | Student health profiles, dispensary clinic visits |
| `audit` | Audit records and teacher grade modification telemetry |
| `integration` | Outbox and offline synchronization |

These schemas organize the database. They do not represent separate databases or microservices.

---

## 4. Common Database Standards

### 4.1 Primary Keys

All main business entities must use UUID primary keys.

Example:

```sql
id UUID PRIMARY KEY DEFAULT gen_random_uuid()
```

Enable the required PostgreSQL extension through a version-controlled migration if the selected UUID generation approach requires it.

### 4.2 Common Tenant-Owned Columns

Tenant-owned entities should include:

- `id`
- `tenant_id`
- `created_at`
- `created_by`
- `updated_at`
- `updated_by`

Branch-owned entities should include:

- `id`
- `tenant_id`
- `branch_id`
- `created_at`
- `created_by`
- `updated_at`
- `updated_by`

Not every table requires every audit column. Append-only or immutable records may use a different structure.

### 4.3 Data Types

| Data | PostgreSQL Type |
| :--- | :--- |
| **Primary key** | UUID |
| **Tenant ID** | UUID |
| **Branch ID** | UUID |
| **Name** | VARCHAR(150) or TEXT |
| **Description** | TEXT |
| **Email** | VARCHAR(255) |
| **Phone** | VARCHAR(30) |
| **Money** | NUMERIC(14,2) |
| **Percentage** | NUMERIC(7,4) |
| **Date** | DATE |
| **Timestamp** | TIMESTAMPTZ |
| **Boolean** | BOOLEAN |
| **Status** | VARCHAR(30) |
| **Structured metadata** | JSONB |

Use `JSONB` only for flexible metadata or configuration. Do not use it as a substitute for relational tables when data requires relationships, constraints, or frequent filtering.

### 4.4 Naming Conventions

- Table names: `snake_case`, plural where appropriate.
- Column names: `snake_case`.
- Primary key: `id`.
- Foreign keys: descriptive names ending in `_id`.
- Tenant ownership: `tenant_id`.
- Branch ownership: `branch_id`.
- Creation timestamp: `created_at`.
- Update timestamp: `updated_at`.
- Unique constraints: `uq_<table>_<columns>`.
- Foreign keys: `fk_<table>_<referenced_table>`.
- Indexes: `ix_<table>_<columns>`.

---

## 5. Core Entity Relationships

The following diagram shows the principal relationships.

```text
PLATFORM
   |
   +-- TENANTS
          |
          +-- USERS / MEMBERSHIPS
          |
          +-- BRANCHES
                 |
                 +-- SCHOOL CONFIGURATION
                 |
                 +-- STUDENTS
                 |      |
                 |      +-- GUARDIANS
                 |      |
                 |      +-- ADMISSIONS
                 |      |
                 |      +-- ENROLLMENTS
                 |              |
                 |              +-- CLASS / SECTION
                 |              |
                 |              +-- ATTENDANCE
                 |              |
                 |              +-- EXAMINATION RESULTS
                 |              |
                 |              +-- FEE ASSIGNMENTS
                 |                      |
                 |                      +-- INVOICES
                 |                              |
                 |                              +-- PAYMENTS
                 |                              +-- ALLOCATIONS
                 |                              +-- REFUNDS
                 |
                 +-- STAFF / TEACHERS
```

Student identity and academic enrollment must remain separate.

A student can have multiple historical enrollments, including enrollments at different branches, without losing the student's identity or historical academic records.

---

## 6. Platform Schema

### 6.1 `platform.tenants`

Stores school organizations registered on the SaaS platform.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `name` | VARCHAR(200) | Required |
| `code` | VARCHAR(50) | Required, unique |
| `legal_name` | VARCHAR(200) | Optional |
| `email` | VARCHAR(255) | Optional |
| `phone` | VARCHAR(30) | Optional |
| `status` | VARCHAR(30) | Required |
| `default_currency` | VARCHAR(3) | Default ETB |
| `created_at` | TIMESTAMPTZ | Required |
| `updated_at` | TIMESTAMPTZ | Required |

Tenant status values:
- `pending`
- `active`
- `suspended`
- `closed`

Tenant codes must be unique.

### 6.2 `platform.tenant_settings`

Stores tenant-level configuration.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | FK |
| `setting_key` | VARCHAR(100) | Required |
| `setting_value` | JSONB | Required |
| `updated_at` | TIMESTAMPTZ | Required |

Constraint:
```sql
UNIQUE (tenant_id, setting_key)
```

Use this table only for settings that genuinely require flexible configuration.

---

## 7. Identity Schema

### 7.1 `identity.users`

Stores platform user identities.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `email` | VARCHAR(255) | Nullable |
| `phone` | VARCHAR(30) | Nullable |
| `password_hash` | TEXT | Nullable for external identity |
| `first_name` | VARCHAR(100) | Required |
| `middle_name` | VARCHAR(100) | Optional |
| `last_name` | VARCHAR(100) | Required |
| `status` | VARCHAR(30) | Required |
| `email_verified` | BOOLEAN | Default false |
| `phone_verified` | BOOLEAN | Default false |
| `created_at` | TIMESTAMPTZ | Required |
| `updated_at` | TIMESTAMPTZ | Required |

Identity records are platform-level identities.

They must not contain tenant-specific student, staff, or financial data.

Email and phone uniqueness rules must be defined according to the chosen authentication model. If a user may share a family email address, do not assume that every guardian email must be globally unique.

### 7.2 `identity.tenant_memberships`

Connects users to tenants.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | FK |
| `user_id` | UUID | FK |
| `status` | VARCHAR(30) | Required |
| `joined_at` | TIMESTAMPTZ | Optional |
| `created_at` | TIMESTAMPTZ | Required |

Constraint:
```sql
UNIQUE (tenant_id, user_id)
```

### 7.3 `identity.user_branch_access`

Stores explicit branch access.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `user_id` | UUID | Required |
| `created_at` | TIMESTAMPTZ | Required |

Constraints:
```sql
UNIQUE (tenant_id, branch_id, user_id)

FOREIGN KEY (tenant_id, branch_id)
REFERENCES school.branches (tenant_id, id)
```

The user must also have a valid membership in the same tenant.

### 7.4 `identity.roles`

Stores tenant-specific roles and approved platform-level roles.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Nullable for platform roles |
| `name` | VARCHAR(100) | Required |
| `code` | VARCHAR(80) | Required |
| `description` | TEXT | Optional |
| `is_system_role` | BOOLEAN | Default false |
| `created_at` | TIMESTAMPTZ | Required |

Role codes must be unique within their ownership scope.

Platform roles and tenant roles must have clearly separated ownership rules.

### 7.5 `identity.permissions`

Stores permission definitions.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `code` | VARCHAR(120) | Required, unique |
| `module` | VARCHAR(80) | Required |
| `description` | TEXT | Optional |

Examples:
- `student.view`
- `student.create`
- `student.update`
- `student.archive`
- `admission.view`
- `admission.approve`
- `admission.reject`
- `attendance.view`
- `attendance.create`
- `attendance.correct`
- `fee.view`
- `fee.create_invoice`
- `fee.confirm_payment`
- `fee.approve_refund`
- `examination.enter_marks`
- `examination.approve_results`
- `examination.publish_results`

### 7.6 `identity.role_permissions`

Connects roles to permissions.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `role_id` | UUID | FK |
| `permission_id` | UUID | FK |

Primary key:
```sql
(role_id, permission_id)
```

### 7.7 `identity.user_roles`

Assigns roles to users within a tenant and optionally a branch.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `user_id` | UUID | Required |
| `role_id` | UUID | Required |
| `branch_id` | UUID | Nullable |
| `created_at` | TIMESTAMPTZ | Required |

A role assignment must be valid for the user's tenant membership.

Branch-scoped role assignments must reference a branch belonging to that tenant.

A tenant-wide role must be explicitly distinguishable from a branch-scoped role.

### 7.8 Authentication & Session Architecture

Authentication lifecycle, session management, credentials, password hashes, and refresh tokens are managed by the dedicated **Better Auth Service** (Node.js/PostgreSQL with Drizzle ORM, Organizations, and JWT plugins) in its own dedicated schema:
- Better Auth manages credentials, Argon2id password hashes, sessions, refresh tokens, password resets, and organization memberships (`tenant_id`).
- Better Auth issues cryptographically signed JWTs (RS256/EdDSA) with JWKS public key rotation.
- **ASP.NET Core (`FidelHub.Api`)** functions strictly as a **stateless JWT Resource Server** (`JwtBearer` via JWKS validation). It maintains zero local session state.
- ASP.NET Core relies on the local PostgreSQL tables defined above (`identity.user_branch_access`, `identity.roles`, `identity.permissions`, `identity.role_permissions`, and `identity.user_roles`) for campus branch scoping and granular RBAC/PBAC authorization.

---

## 8. School Schema

### 8.1 `school.branches`

Stores independently administered campuses.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `name` | VARCHAR(200) | Required |
| `code` | VARCHAR(50) | Required |
| `address` | TEXT | Optional |
| `phone` | VARCHAR(30) | Optional |
| `email` | VARCHAR(255) | Optional |
| `currency` | VARCHAR(3) | Default ETB |
| `timezone` | VARCHAR(80) | Required |
| `status` | VARCHAR(30) | Required |
| `created_at` | TIMESTAMPTZ | Required |
| `updated_at` | TIMESTAMPTZ | Required |

Constraints:
```sql
UNIQUE (tenant_id, id)
UNIQUE (tenant_id, code)
```

Each branch belongs to exactly one tenant.

### 8.2 `school.branch_settings`

Stores branch-level configuration.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `setting_key` | VARCHAR(100) | Required |
| `setting_value` | JSONB | Required |
| `updated_at` | TIMESTAMPTZ | Required |

Constraint:
```sql
UNIQUE (tenant_id, branch_id, setting_key)
```

### 8.3 `school.academic_calendars`

Stores calendar configuration for each branch.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `calendar_name` | VARCHAR(100) | Required |
| `calendar_type` | VARCHAR(30) | Required |
| `is_default` | BOOLEAN | Default false |
| `created_at` | TIMESTAMPTZ | Required |

Calendar type must support the approved Ethiopian and Gregorian calendar requirements.

The authoritative date values remain Gregorian.

---

## 9. Student Schema

### 9.1 `students.students`

Stores student identity and personal profile.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `current_branch_id` | UUID | Nullable (FK to school.branches) |
| `admission_number` | VARCHAR(50) | Required within tenant |
| `first_name` | VARCHAR(100) | Required |
| `father_name` | VARCHAR(100) | Required |
| `grandfather_name` | VARCHAR(100) | Required |
| `gender` | VARCHAR(20) | Optional |
| `date_of_birth` | DATE | Optional |
| `phone` | VARCHAR(30) | Optional |
| `email` | VARCHAR(255) | Optional |
| `address` | TEXT | Optional |
| `status` | VARCHAR(30) | Required |
| `created_at` | TIMESTAMPTZ | Required |
| `updated_at` | TIMESTAMPTZ | Required |

Constraints:
```sql
UNIQUE (tenant_id, id)
UNIQUE (tenant_id, admission_number)
```

A student identity is tenant-owned, not branch-owned.

`current_branch_id` provides fast campus filtering and current branch directory lookups, while full historical branch and class progression is preserved in `academic.student_enrollments`.

This allows the same student identity to be preserved when the student transfers between branches belonging to the same tenant.

A transfer between unrelated tenants requires a separate, controlled process. Do not silently reuse one tenant's student identity in another tenant.

### 9.2 `students.guardians`

Stores guardian identity and contact information within a tenant.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `first_name` | VARCHAR(100) | Required |
| `middle_name` | VARCHAR(100) | Optional |
| `last_name` | VARCHAR(100) | Required |
| `phone` | VARCHAR(30) | Required |
| `email` | VARCHAR(255) | Optional |
| `address` | TEXT | Optional |
| `created_at` | TIMESTAMPTZ | Required |

### 9.3 `students.student_guardians`

Connects students to guardians.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `tenant_id` | UUID | Required |
| `student_id` | UUID | Required |
| `guardian_id` | UUID | Required |
| `relationship` | VARCHAR(50) | Required |
| `is_primary` | BOOLEAN | Default false |
| `is_emergency_contact` | BOOLEAN | Default false |
| `can_collect_student` | BOOLEAN | Default false |

Primary key:
```sql
(tenant_id, student_id, guardian_id)
```

Foreign keys must ensure the student and guardian belong to the same tenant.

---

## 10. Admissions Schema

### 10.1 `admissions.applications`

Stores admission applications.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `application_number` | VARCHAR(50) | Required |
| `academic_year_id` | UUID | Required |
| `student_id` | UUID | Nullable until linked |
| `applicant_first_name` | VARCHAR(100) | Required |
| `applicant_father_name` | VARCHAR(100) | Required |
| `applicant_grandfather_name` | VARCHAR(100) | Required |
| `date_of_birth` | DATE | Optional |
| `requested_class_id` | UUID | Required |
| `status` | VARCHAR(30) | Required |
| `submitted_at` | TIMESTAMPTZ | Optional |
| `reviewed_by` | UUID | Optional |
| `reviewed_at` | TIMESTAMPTZ | Optional |
| `decision_notes` | TEXT | Optional |
| `created_at` | TIMESTAMPTZ | Required |

Constraint:
```sql
UNIQUE (tenant_id, branch_id, application_number)
```

Admission status:
- `draft`
- `submitted`
- `under_review`
- `approved`
- `rejected`
- `withdrawn`

The academic year and requested class must belong to the same tenant and branch.

### 10.2 `admissions.application_documents`

Stores admission document metadata.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `application_id` | UUID | Required |
| `document_type` | VARCHAR(80) | Required |
| `file_name` | VARCHAR(255) | Required |
| `storage_key` | TEXT | Required |
| `uploaded_by` | UUID | Required |
| `uploaded_at` | TIMESTAMPTZ | Required |

File contents are stored in approved object storage, not in this table.

---

## 11. Academic Schema

### 11.1 `academic.academic_years`

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `name` | VARCHAR(100) | Required |
| `start_date` | DATE | Required |
| `end_date` | DATE | Required |
| `status` | VARCHAR(30) | Required |
| `created_at` | TIMESTAMPTZ | Required |

Constraints:
```sql
UNIQUE (tenant_id, branch_id, id)
CHECK (end_date >= start_date)
```

Academic year dates are stored in Gregorian format.

### 11.2 `academic.terms`

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `academic_year_id` | UUID | Required |
| `name` | VARCHAR(100) | Required |
| `start_date` | DATE | Required |
| `end_date` | DATE | Required |
| `sequence` | INTEGER | Required |
| `status` | VARCHAR(30) | Required |

Constraints:
- Term must reference an academic year belonging to the same tenant and branch.
- Term dates must fall within the academic year's dates.
- Term end date must not precede its start date.
- Term sequence must be unique within the academic year.

### 11.3 `academic.classes`

Represents grade or class levels.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `name` | VARCHAR(100) | Required |
| `code` | VARCHAR(50) | Required |
| `sequence` | INTEGER | Required |
| `status` | VARCHAR(30) | Required |

Constraints:
```sql
UNIQUE (tenant_id, branch_id, id)
UNIQUE (tenant_id, branch_id, code)
```

Examples:
- KG 1
- KG 2
- Grade 1
- Grade 2
- Grade 9
- Grade 10

### 11.4 `academic.sections`

Represents sections within classes.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `class_id` | UUID | Required |
| `name` | VARCHAR(50) | Required |
| `capacity` | INTEGER | Optional |
| `status` | VARCHAR(30) | Required |

Important constraints:
```sql
UNIQUE (tenant_id, branch_id, class_id, id)
UNIQUE (tenant_id, branch_id, class_id, name)

FOREIGN KEY (tenant_id, branch_id, class_id)
REFERENCES academic.classes (tenant_id, branch_id, id)
```

### 11.5 `academic.student_enrollments`

Connects students to branches, academic years, classes, and sections.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `student_id` | UUID | Required |
| `academic_year_id` | UUID | Required |
| `class_id` | UUID | Required |
| `section_id` | UUID | Required |
| `enrollment_date` | DATE | Required |
| `status` | VARCHAR(30) | Required |
| `previous_enrollment_id` | UUID | Optional |
| `created_at` | TIMESTAMPTZ | Required |

Important constraints:
```sql
UNIQUE (tenant_id, branch_id, id)
UNIQUE (tenant_id, branch_id, id, student_id)

FOREIGN KEY (tenant_id, student_id)
REFERENCES students.students (tenant_id, id)

FOREIGN KEY (tenant_id, branch_id, academic_year_id)
REFERENCES academic.academic_years (tenant_id, branch_id, id)

FOREIGN KEY (tenant_id, branch_id, class_id)
REFERENCES academic.classes (tenant_id, branch_id, id)

FOREIGN KEY (tenant_id, branch_id, class_id, section_id)
REFERENCES academic.sections (tenant_id, branch_id, class_id, id)
```

Enrollment statuses:
- `active`
- `completed`
- `transferred`
- `withdrawn`
- `suspended`

Do not delete historical enrollments when a student changes class or branch.
A new enrollment or controlled transfer record must preserve the previous enrollment history.

### 11.6 `academic.subjects`

Stores subjects configured for a branch.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `name` | VARCHAR(120) | Required |
| `code` | VARCHAR(50) | Required |
| `description` | TEXT | Optional |
| `status` | VARCHAR(30) | Required |

### 11.7 `academic.class_subjects`

Connects subjects to classes.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `class_id` | UUID | Required |
| `subject_id` | UUID | Required |
| `weekly_periods` | INTEGER | Optional |

Prevent duplicate subject assignments for the same class.

---

## 12. Staff Schema

### 12.1 `staff.staff_members`

Stores branch-specific staff records.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `employee_number` | VARCHAR(50) | Required |
| `user_id` | UUID | Nullable |
| `first_name` | VARCHAR(100) | Required |
| `middle_name` | VARCHAR(100) | Optional |
| `last_name` | VARCHAR(100) | Required |
| `phone` | VARCHAR(30) | Optional |
| `email` | VARCHAR(255) | Optional |
| `employment_type` | VARCHAR(50) | Optional |
| `status` | VARCHAR(30) | Required |
| `created_at` | TIMESTAMPTZ | Required |

Constraints:
```sql
UNIQUE (tenant_id, branch_id, id)
UNIQUE (tenant_id, branch_id, employee_number)
```

A staff member may have a linked system user account, but a user account is not mandatory for every staff record.

### 12.2 `staff.teacher_assignments`

Stores teacher-to-class and subject assignments.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK |
| `tenant_id` | UUID | Required |
| `branch_id` | UUID | Required |
| `staff_member_id` | UUID | Required |
| `academic_year_id` | UUID | Required |
| `class_id` | UUID | Required |
| `section_id` | UUID | Required |
| `subject_id` | UUID | Required |
| `status` | VARCHAR(30) | Required |

All referenced records must belong to the same tenant and branch.

---

## 13. Attendance Schema

### 13.1 `attendance.sessions`

Represents a class attendance session.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| class_id | UUID | Required |
| section_id | UUID | Required |
| session_date | DATE | Required |
| session_type | VARCHAR(30) | Required (HOMEROOM, PERIOD) |
| period_number | INTEGER | Nullable (Required if PERIOD) |
| 	aken_by | UUID | Required (FK to staff) |
| status | VARCHAR(30) | Required (SUBMITTED, LOCKED) |
| created_at | TIMESTAMPTZ | Required |

Constraints:
UNIQUE (tenant_id, branch_id, class_id, section_id, session_date, session_type, period_number)

### 13.2 ttendance.records

Stores attendance record for each student in a session.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| session_id | UUID | Required (FK to attendance.sessions) |
| student_id | UUID | Required (FK to students.students) |
| status | VARCHAR(20) | Required (PRESENT, ABSENT_UNEXCUSED, ABSENT_EXCUSED, LATE, HALF_DAY) |
| rrival_time | TIME | Nullable |
| 
emarks | TEXT | Nullable |
| created_at | TIMESTAMPTZ | Required |

Constraints:
UNIQUE (tenant_id, session_id, student_id)
FOREIGN KEY (tenant_id, branch_id, session_id) REFERENCES attendance.sessions (tenant_id, branch_id, id)
FOREIGN KEY (tenant_id, student_id) REFERENCES students.students (tenant_id, id)

### 13.3 ttendance.corrections

Audit log of any post-submission attendance modifications.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| 
ecord_id | UUID | Required (FK to attendance.records) |
| previous_status | VARCHAR(20) | Required |
| 
ew_status | VARCHAR(20) | Required |
| 
eason | TEXT | Required |
| uthorized_by | UUID | Required |
| corrected_at | TIMESTAMPTZ | Required |

---

## 14. Examination & CBT Schema

### 14.1 examinations.question_banks

Containers for subject/department questions.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| 
ame | VARCHAR(150) | Required |
| subject_id | UUID | Required (FK to academic.subjects) |
| class_id | UUID | Required (FK to academic.classes) |
| description | TEXT | Nullable |
| is_active | BOOLEAN | Default true |
| created_at | TIMESTAMPTZ | Required |

### 14.2 examinations.questions

Individual questions supporting rich formatting, KaTeX, and Bloom's taxonomy.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| question_bank_id | UUID | Required (FK to question_banks) |
| question_text | TEXT | Required (Supports Markdown & KaTeX formulas) |
| question_type | VARCHAR(30) | Required (MCQ, MULTI_SELECT, TRUE_FALSE, FILL_BLANK, THEORY) |
| difficulty | VARCHAR(20) | Required (EASY, MEDIUM, HARD) |
| cognitive_level | VARCHAR(30) | Optional (REMEMBERING, UNDERSTANDING, APPLYING, ANALYZING) |
| 	opic_tag | VARCHAR(100) | Optional |
| points | NUMERIC(5,2) | Required (Default 1.00) |
| media_url | TEXT | Nullable (Diagram / Image attachment) |
| 
ubric_explanation| TEXT | Nullable (Grading notes for theory questions) |
| created_by | UUID | Required |
| created_at | TIMESTAMPTZ | Required |

### 14.3 examinations.question_options

Options for objective questions (MCQ / Multi-Select / True-False).

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| question_id | UUID | Required (FK to questions) |
| option_text | TEXT | Required |
| option_order | INTEGER | Required |
| is_correct | BOOLEAN | Required |
| explanation | TEXT | Nullable |

Constraint:
UNIQUE (tenant_id, question_id, option_order)

### 14.4 examinations.exam_papers

Assembled examination papers ready for sitting.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| cademic_year_id | UUID | Required |
| 	erm_id | UUID | Required |
| subject_id | UUID | Required |
| class_id | UUID | Required |
| 	itle | VARCHAR(200) | Required |
| 	otal_marks | NUMERIC(6,2) | Required |
| pass_marks | NUMERIC(6,2) | Required |
| duration_minutes | INTEGER | Required |
| shuffle_questions | BOOLEAN | Default true |
| shuffle_options | BOOLEAN | Default true |
| status | VARCHAR(30) | Required (DRAFT, PUBLISHED, ARCHIVED) |
| created_at | TIMESTAMPTZ | Required |

### 14.5 examinations.exam_schedules

Scheduling exam paper sessions for classes or specific venues.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| exam_paper_id | UUID | Required (FK to exam_papers) |
| section_id | UUID | Nullable (If branch-wide, null) |
| start_time | TIMESTAMPTZ | Required |
| end_time | TIMESTAMPTZ | Required |
| enue_identifier | VARCHAR(100) | Optional |
| is_time_locked | BOOLEAN | Default true |
| package_checksum | VARCHAR(64) | SHA-256 for offline venue validation |

### 14.6 examinations.cbt_sessions

Individual student exam sitting session.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK (Server ID or Client UUID generated offline) |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| exam_paper_id | UUID | Required |
| student_id | UUID | Required |
| session_token | UUID | Unique session token |
| started_at | TIMESTAMPTZ | Required |
| submitted_at | TIMESTAMPTZ | Nullable until finished |
| 	ime_spent_seconds| INTEGER | Required Default 0 |
| is_final | BOOLEAN | Default false |
| status | VARCHAR(30) | Required (IN_PROGRESS, SUBMITTED, AUTO_SUBMITTED, GRADED) |
| 	otal_score | NUMERIC(6,2) | Nullable until graded |
| sync_status | VARCHAR(20) | Default 'SYNCED' |
| enue_server_id | VARCHAR(100) | Nullable |

Constraint:
UNIQUE (tenant_id, exam_paper_id, student_id)

### 14.7 examinations.student_answers

Student responses to individual questions with autosave tracking.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| session_id | UUID | Required (FK to cbt_sessions) |
| question_id | UUID | Required (FK to questions) |
| selected_option_ids| JSONB | Nullable (Array of option UUIDs) |
| 	ext_answer | TEXT | Nullable (For theory questions) |
| is_flagged_review | BOOLEAN | Default false |
| is_auto_graded | BOOLEAN | Default false |
| score_awarded | NUMERIC(5,2) | Nullable |
| grader_feedback | TEXT | Nullable |
| nswered_at | TIMESTAMPTZ | Required |
| utosave_version | INTEGER | Default 1 |

Constraint:
UNIQUE (tenant_id, session_id, question_id)

---

## 15. Academic Curriculum Extensions & Timetables

### 15.1 cademic.lesson_plans

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| staff_member_id | UUID | Required |
| subject_id | UUID | Required |
| class_id | UUID | Required |
| week_number | INTEGER | Required |
| 	opic_title | VARCHAR(200) | Required |
| learning_objectives| TEXT | Required |
| 	eaching_methodology| TEXT | Required |
| status | VARCHAR(30) | Required (DRAFT, SUBMITTED, APPROVED, REVISION_REQUESTED) |
| 
eviewed_by | UUID | Nullable |
| 
eviewed_at | TIMESTAMPTZ | Nullable |
| 
eview_comments | TEXT | Nullable |

### 15.2 cademic.timetables

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| cademic_year_id | UUID | Required |
| 	erm_id | UUID | Required |
| class_id | UUID | Required |
| section_id | UUID | Required |
| day_of_week | INTEGER | Required (1=Monday .. 5=Friday, 6=Saturday) |
| period_number | INTEGER | Required |
| start_time | TIME | Required |
| end_time | TIME | Required |
| subject_id | UUID | Required |
| 	eacher_id | UUID | Required |
| 
oom_number | VARCHAR(50) | Optional |

Constraints:
UNIQUE (tenant_id, branch_id, class_id, section_id, day_of_week, period_number)
UNIQUE (tenant_id, branch_id, teacher_id, day_of_week, period_number) -- Prevents teacher double booking

### 15.3 cademic.affective_traits

Behavioral and character development evaluation areas.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| 	rait_name | VARCHAR(100) | Required (e.g. Punctuality, Neatness, Teamwork, Leadership) |
| 	rait_category | VARCHAR(50) | Required (AFFECTIVE, PSYCHOMOTOR) |
| description | TEXT | Optional |

### 15.4 cademic.student_trait_scores

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| cademic_year_id | UUID | Required |
| 	erm_id | UUID | Required |
| student_id | UUID | Required |
| 	rait_id | UUID | Required |
| 
ating_score | INTEGER | Required (Scale 1 to 5) |
| evaluated_by | UUID | Required (Teacher) |

Constraint:
UNIQUE (tenant_id, branch_id, term_id, student_id, trait_id)

### 15.5 cademic.gradebooks & 
eport_cards

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| cademic_year_id | UUID | Required |
| 	erm_id | UUID | Required |
| student_id | UUID | Required |
| class_id | UUID | Required |
| section_id | UUID | Required |
| 	otal_marks | NUMERIC(7,2) | Required |
| verage_score | NUMERIC(5,2) | Required |
| gpa | NUMERIC(4,2) | Optional |
| class_rank | INTEGER | Optional |
| section_rank | INTEGER | Optional |
| principal_remark | TEXT | Optional |
| erification_hash | VARCHAR(64) | SHA-256 QR code digest |
| is_published | BOOLEAN | Default false |
| published_at | TIMESTAMPTZ | Nullable |

---

## 16. Fee & Invoicing Schema (No Full General Ledger)

### 16.1 inance.fee_categories

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| 
ame | VARCHAR(100) | Required (Tuition, Registration, Lab, Transport) |
| code | VARCHAR(50) | Required |
| description | TEXT | Optional |
| is_recurring | BOOLEAN | Default true |

Constraint:
UNIQUE (tenant_id, branch_id, code)

### 16.2 inance.fee_structures

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| cademic_year_id | UUID | Required |
| 	erm_id | UUID | Required |
| class_id | UUID | Required |
| 
ame | VARCHAR(150) | Required |
| 	otal_base_amount | NUMERIC(14,2)| Required |
| status | VARCHAR(30) | Required (ACTIVE, ARCHIVED) |

### 16.3 inance.fee_structure_lines

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ee_structure_id | UUID | Required (FK to fee_structures) |
| ee_category_id | UUID | Required (FK to fee_categories) |
| mount | NUMERIC(14,2)| Required |
| is_mandatory | BOOLEAN | Default true |

### 16.4 inance.student_fee_assignments

Captures an immutable snapshot of student fee commitments (with approved scholarships/discounts).

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| student_id | UUID | Required (FK to students) |
| cademic_year_id | UUID | Required |
| 	erm_id | UUID | Required |
| ee_structure_id | UUID | Required |
| discount_percentage| NUMERIC(5,2)| Default 0.00 |
| discount_reason | VARCHAR(255) | Optional |
| custom_net_amount | NUMERIC(14,2)| Required (Frozen snapshot) |
| pproved_by | UUID | Optional |
| created_at | TIMESTAMPTZ | Required |

Constraint:
UNIQUE (tenant_id, branch_id, term_id, student_id)

### 16.5 inance.invoices

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| student_id | UUID | Required (FK to students) |
| cademic_year_id | UUID | Required |
| 	erm_id | UUID | Required |
| invoice_number | VARCHAR(50) | Required |
| due_date | DATE | Required |
| subtotal_amount | NUMERIC(14,2)| Required |
| discount_amount | NUMERIC(14,2)| Default 0.00 |
| 	otal_amount | NUMERIC(14,2)| Required |
| paid_amount | NUMERIC(14,2)| Default 0.00 |
| status | VARCHAR(30) | Required (DRAFT, POSTED, PARTIAL, PAID, CANCELLED) |
| posted_at | TIMESTAMPTZ | Nullable |
| created_at | TIMESTAMPTZ | Required |

Constraints:
UNIQUE (tenant_id, branch_id, invoice_number)
CHECK (paid_amount <= total_amount)

### 16.6 inance.invoice_items

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| invoice_id | UUID | Required (FK to invoices) |
| ee_category_id | UUID | Required |
| description | VARCHAR(200) | Required |
| quantity | INTEGER | Default 1 |
| unit_price | NUMERIC(14,2)| Required |
| line_total | NUMERIC(14,2)| Required |

### 16.7 inance.payments

Records incoming payments against invoices with idempotency protection.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| invoice_id | UUID | Required (FK to invoices) |
| student_id | UUID | Required |
| mount | NUMERIC(14,2)| Required |
| payment_date | DATE | Required |
| payment_method | VARCHAR(30) | Required (CASH, TELEBIRR, CBE_BIRR, BANK_SLIP) |
| 
eference_number | VARCHAR(100) | Nullable |
| idempotency_key | UUID | Required, UNIQUE |
| cashier_user_id | UUID | Required |
| created_at | TIMESTAMPTZ | Required |

Constraints:
UNIQUE (tenant_id, idempotency_key)
CHECK (amount > 0)

### 16.8 inance.receipts

Sequential official receipts.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| payment_id | UUID | Required (FK to payments) |
| 
eceipt_number | VARCHAR(50) | Required |
| issued_at | TIMESTAMPTZ | Required |
| is_cancelled | BOOLEAN | Default false |

Constraint:
UNIQUE (tenant_id, branch_id, receipt_number)

---

## 17. Health Schema

### 17.1 health.student_health_profiles

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| student_id | UUID | Required, UNIQUE |
| lood_group | VARCHAR(10) | Optional |
| llergies | TEXT | Optional |
| chronic_conditions| TEXT | Optional |
| emergency_instructions| TEXT | Optional |
| preferred_hospital| VARCHAR(200) | Optional |

### 17.2 health.clinic_visits

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| student_id | UUID | Required |
| isit_date | TIMESTAMPTZ | Required |
| symptoms | TEXT | Required |
| ction_taken | TEXT | Required |
| 	emperature_celsius| NUMERIC(4,1)| Optional |
| parent_notified | BOOLEAN | Default false |
| ttended_by | UUID | Required (Staff) |

---

## 18. Audit & Integration Schema

### 18.1 udit.activity_logs

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Nullable |
| user_id | UUID | Nullable |
| module | VARCHAR(50) | Required |
| ction | VARCHAR(100) | Required |
| entity_name | VARCHAR(100) | Required |
| entity_id | UUID | Nullable |
| old_values | JSONB | Nullable |
| 
ew_values | JSONB | Nullable |
| ip_address | VARCHAR(45) | Nullable |
| occurred_at | TIMESTAMPTZ | Required |

### 18.2 integration.outbox_messages

Transactional Outbox pattern message queue.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| event_type | VARCHAR(150) | Required |
| payload | JSONB | Required |
| occurred_at | TIMESTAMPTZ | Required |
| processed_at | TIMESTAMPTZ | Nullable |
| 
etry_count | INTEGER | Default 0 |
| error | TEXT | Nullable |

Index:
CREATE INDEX ix_outbox_unprocessed ON integration.outbox_messages (occurred_at) WHERE processed_at IS NULL;

### 18.3 integration.offline_cbt_sync_batches

Tracks offline CBT batches ingested from local exam venue servers.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK |
| 	enant_id | UUID | Required |
| ranch_id | UUID | Required |
| enue_server_id | VARCHAR(100) | Required |
| atch_token | UUID | Required, UNIQUE |
| exam_paper_id | UUID | Required |
| sessions_count | INTEGER | Required |
| 
eceived_at | TIMESTAMPTZ | Required |
| processed_at | TIMESTAMPTZ | Nullable |
| status | VARCHAR(30) | Required (RECEIVED, PROCESSED, PARTIAL_ERRORS) |


---

## 19. Library Schema (`library`)

Supports physical book cataloging, circulation loans, overdue fines, and digital educational media.

### 19.1 library.books

Physical and digital books in the school library catalog.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Required, FK -> school.branches(id) |
| isbn | VARCHAR(30) | Nullable |
| title | VARCHAR(255) | Required |
| author | VARCHAR(200) | Required |
| category | VARCHAR(100) | Required (Textbook, Fiction, Science, Reference) |
| publisher | VARCHAR(150) | Nullable |
| edition | VARCHAR(50) | Nullable |
| rack_location | VARCHAR(50) | Nullable (e.g., 'Shelf B4-3') |
| total_copies | INTEGER | Required, Default 1 |
| available_copies | INTEGER | Required, Default 1 |
| is_active | BOOLEAN | Required, Default true |
| created_at | TIMESTAMPTZ | Required |
| updated_at | TIMESTAMPTZ | Required |

Indexes:
```sql
CREATE INDEX ix_library_books_branch ON library.books (tenant_id, branch_id);
CREATE INDEX ix_library_books_search ON library.books (tenant_id, title, author);
```

### 19.2 library.book_loans

Records checkout and return transactions of books to students or staff.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Required, FK -> school.branches(id) |
| book_id | UUID | Required, FK -> library.books(id) |
| borrower_type | VARCHAR(20) | Required ('STUDENT', 'STAFF') |
| borrower_id | UUID | Required (Student or Staff UUID) |
| borrowed_at | TIMESTAMPTZ | Required |
| due_date | DATE | Required |
| returned_at | TIMESTAMPTZ | Nullable |
| return_condition | VARCHAR(30) | Nullable ('GOOD', 'DAMAGED', 'LOST') |
| status | VARCHAR(30) | Required ('ACTIVE', 'RETURNED', 'OVERDUE', 'LOST') |
| notes | TEXT | Nullable |
| created_by | UUID | Required |

Constraints & Indexes:
```sql
CREATE INDEX ix_book_loans_active ON library.book_loans (tenant_id, branch_id, status) WHERE status = 'ACTIVE';
CREATE INDEX ix_book_loans_borrower ON library.book_loans (tenant_id, borrower_id);
```

### 19.3 library.overdue_fines

Tracks accrued overdue book fines integrated with the student fee statement.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Required, FK -> school.branches(id) |
| loan_id | UUID | Required, FK -> library.book_loans(id) |
| student_id | UUID | Required, FK -> students.students(id) |
| overdue_days | INTEGER | Required |
| daily_rate | NUMERIC(14,2) | Required |
| fine_amount | NUMERIC(14,2) | Required |
| status | VARCHAR(30) | Required ('UNPAID', 'PAID', 'WAIVED') |
| waiver_reason | TEXT | Nullable |
| waived_by | UUID | Nullable |
| created_at | TIMESTAMPTZ | Required |
| settled_at | TIMESTAMPTZ | Nullable |

### 19.4 library.digital_resources

Online downloadable curriculum materials, PDF textbooks, and instructional video links.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Nullable (null = tenant-wide) |
| title | VARCHAR(255) | Required |
| subject_id | UUID | Nullable, FK -> academic.subjects(id) |
| grade_level | VARCHAR(50) | Nullable |
| resource_type | VARCHAR(30) | Required ('PDF_TEXTBOOK', 'SYLLABUS', 'VIDEO_LINK', 'PAST_PAPER') |
| file_url | TEXT | Required |
| file_size_bytes | BIGINT | Nullable |
| is_public | BOOLEAN | Required, Default false |
| created_at | TIMESTAMPTZ | Required |

---

## 20. Facilities & Laboratory Schema (`facilities`)

Manages science laboratories, ICT computer labs, equipment condition, consumables, and booking schedules.

### 20.1 facilities.equipment

Registry of apparatus, computers, projectors, and laboratory assets.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Required, FK -> school.branches(id) |
| asset_tag | VARCHAR(50) | Required, UNIQUE per branch |
| name | VARCHAR(200) | Required |
| facility_type | VARCHAR(50) | Required ('SCIENCE_LAB', 'ICT_LAB', 'WORKSHOP') |
| category | VARCHAR(100) | Required (Microscope, PC, Bunsen Burner, Projector) |
| condition_status | VARCHAR(30) | Required ('FUNCTIONAL', 'DAMAGED', 'UNDER_MAINTENANCE', 'DECOMMISSIONED') |
| quantity | INTEGER | Required, Default 1 |
| room_number | VARCHAR(50) | Nullable |
| acquired_date | DATE | Nullable |
| notes | TEXT | Nullable |
| updated_at | TIMESTAMPTZ | Required |

### 20.2 facilities.lab_bookings

Schedules laboratory and ICT room usage to prevent timetable collisions.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Required, FK -> school.branches(id) |
| facility_name | VARCHAR(100) | Required ('Physics Lab', 'ICT Lab 1') |
| `teacher_user_id` | UUID | Required, FK -> staff.staff_members(id) |
| section_id | UUID | Required, FK -> academic.sections(id) |
| subject_id | UUID | Required, FK -> academic.subjects(id) |
| booking_date | DATE | Required |
| period_number | INTEGER | Required |
| status | VARCHAR(30) | Required ('PENDING', 'CONFIRMED', 'CANCELLED', 'COMPLETED') |
| purpose | TEXT | Required |
| safety_checklist_verified | BOOLEAN | Required, Default false |
| created_at | TIMESTAMPTZ | Required |

Constraint:
```sql
CONSTRAINT uq_facility_booking UNIQUE (tenant_id, branch_id, facility_name, booking_date, period_number);
```

### 20.3 facilities.incidents

Breakage, chemical spills, or equipment damage incidents logged during practical sessions.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Required, FK -> school.branches(id) |
| booking_id | UUID | Nullable, FK -> facilities.lab_bookings(id) |
| equipment_id | UUID | Nullable, FK -> facilities.equipment(id) |
| student_id | UUID | Nullable, FK -> students.students(id) |
| reported_by | UUID | Required |
| incident_date | TIMESTAMPTZ | Required |
| description | TEXT | Required |
| replacement_cost | NUMERIC(14,2) | Nullable |
| fee_invoice_id | UUID | Nullable, FK -> finance.invoices(id) |
| resolution_status | VARCHAR(30) | Required ('OPEN', 'RESOLVED', 'BILLED') |

---

## 21. Health & Student Clinic Schema (`health`)

Student medical records, verified allergies, and daily clinic dispensary visit logs.

### 21.1 `health.student_health_profiles`

Comprehensive health and emergency records for enrolled students.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK DEFAULT gen_random_uuid() |
| `tenant_id` | UUID | Required, FK -> platform.tenants(id) |
| `student_id` | UUID | Required, UNIQUE, FK -> students.students(id) |
| `blood_group` | VARCHAR(5) | Nullable ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-') |
| `allergies` | TEXT[] | Nullable (e.g., ARRAY['Penicillin', 'Peanuts']) |
| `chronic_conditions` | TEXT[] | Nullable (e.g., ARRAY['Asthma', 'Epilepsy']) |
| `dietary_restrictions` | TEXT | Nullable |
| `emergency_instructions` | TEXT | Nullable |
| `preferred_hospital` | VARCHAR(200) | Nullable |
| `doctor_name` | VARCHAR(150) | Nullable |
| `doctor_phone` | VARCHAR(30) | Nullable |
| `emergency_notes` | TEXT | Nullable |
| `created_at` | TIMESTAMPTZ | Required |
| `updated_at` | TIMESTAMPTZ | Required |

Constraints:
```sql
CONSTRAINT uq_student_health_profiles_tenant_id UNIQUE (tenant_id, id);
CONSTRAINT uq_student_health_profiles_student UNIQUE (tenant_id, student_id);
```

### 21.2 `health.clinic_visits`

Daily dispensary clinic logs tracking sickbay visits, treatments, and parent alerts.

| Column | Type | Rules |
| :--- | :--- | :--- |
| `id` | UUID | PK DEFAULT gen_random_uuid() |
| `tenant_id` | UUID | Required, FK -> platform.tenants(id) |
| `branch_id` | UUID | Required, FK -> school.branches(id) |
| `student_id` | UUID | Required, FK -> students.students(id) |
| `nurse_user_id` | UUID | Required, FK -> staff.staff_members(id) |
| `visit_timestamp` | TIMESTAMPTZ | Required |
| `temperature_celsius` | NUMERIC(4,1) | Nullable |
| `symptoms` | TEXT | Required |
| `triage_level` | VARCHAR(20) | Required ('MILD', 'MODERATE', 'SEVERE') |
| `treatment_administered` | TEXT | Required |
| `rest_bed_assigned` | BOOLEAN | Required, Default false |
| `rest_bed_number` | VARCHAR(20) | Nullable |
| `discharge_timestamp` | TIMESTAMPTZ | Nullable |
| `parent_notified` | BOOLEAN | Required, Default false |
| `notification_sms_id` | UUID | Nullable |
| `created_at` | TIMESTAMPTZ | Required |

Constraints & Indexes:
```sql
CONSTRAINT uq_clinic_visits_tenant_id UNIQUE (tenant_id, id);
CONSTRAINT uq_clinic_visits_branch_id UNIQUE (tenant_id, branch_id, id);
CREATE INDEX ix_clinic_visits_student ON health.clinic_visits (tenant_id, student_id, visit_timestamp DESC);
```

---

## 22. Detailed Staff/Teacher Audit Schema (`audit`)

Captures fine-grained access and modification telemetry for student assessment marks.

### 22.1 audit.teacher_access_logs

High-integrity tamper-evident log capturing marksheet edits and access events.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| branch_id | UUID | Required, FK -> school.branches(id) |
| staff_user_id | UUID | Required |
| action_type | VARCHAR(50) | Required ('VIEW_MARKSHEET', 'ENTER_SCORE', 'UPDATE_SCORE', 'EXPORT_REPORT') |
| student_id | UUID | Nullable, FK -> students.students(id) |
| subject_id | UUID | Nullable, FK -> academic.subjects(id) |
| exam_paper_id | UUID | Nullable, FK -> examinations.exam_papers(id) |
| previous_score | NUMERIC(5,2) | Nullable |
| updated_score | NUMERIC(5,2) | Nullable |
| reason | TEXT | Nullable |
| is_after_lock_deadline | BOOLEAN | Required, Default false |
| ip_address | VARCHAR(45) | Nullable |
| user_agent | TEXT | Nullable |
| occurred_at | TIMESTAMPTZ | Required |

Indexes:
```sql
CREATE INDEX ix_teacher_access_logs ON audit.teacher_access_logs (tenant_id, branch_id, occurred_at DESC);
CREATE INDEX ix_teacher_access_tamper ON audit.teacher_access_logs (tenant_id, is_after_lock_deadline) WHERE is_after_lock_deadline = true;
```

---

## 23. SaaS Platform Subscription & Quota Schema (`platform`)

Enforces multi-tenant tier limits, student caps, and database snapshot exports.

### 23.1 platform.tenant_quotas

Defines plan tiers, student ceilings, and branch allowances.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, UNIQUE, FK -> platform.tenants(id) |
| subscription_plan | VARCHAR(50) | Required ('STARTER', 'STANDARD', 'ENTERPRISE') |
| max_students | INTEGER | Required (e.g., 500, 1500, 5000) |
| max_branches | INTEGER | Required (e.g., 1, 3, 10) |
| storage_quota_gb | INTEGER | Required, Default 20 |
| is_cbt_enabled | BOOLEAN | Required, Default true |
| subscription_expires_at | TIMESTAMPTZ | Nullable |
| updated_at | TIMESTAMPTZ | Required |

### 23.2 platform.tenant_backups

Records tenant-isolated database and media backup archives.

| Column | Type | Rules |
| :--- | :--- | :--- |
| id | UUID | PK DEFAULT gen_random_uuid() |
| tenant_id | UUID | Required, FK -> platform.tenants(id) |
| backup_type | VARCHAR(30) | Required ('DAILY_SCHEDULED', 'MANUAL_EXPORT') |
| file_size_bytes | BIGINT | Required |
| storage_path | TEXT | Required |
| checksum_sha256 | VARCHAR(64) | Required |
| expires_at | TIMESTAMPTZ | Required |
| created_at | TIMESTAMPTZ | Required |
