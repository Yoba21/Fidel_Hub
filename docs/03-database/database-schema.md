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
| **ORM** | Entity Framework Core 10 |
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
| `school` | Branches and school configuration |
| `students` | Student profiles and guardians |
| `admissions` | Admission applications |
| `academic` | Academic years, classes, sections, and enrollment |
| `staff` | Staff and teacher records |
| `attendance` | Attendance sessions and records |
| `examinations` | Assessments, marks, and results |
| `finance` | Fees, invoices, payments, and refunds |
| `audit` | Audit records |
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

### 7.8 Authentication Session Tables

The identity module must also support secure refresh-token and session management.

Recommended tables:
- `identity.refresh_tokens`
- `identity.user_sessions`
- `identity.password_reset_tokens`

Refresh-token records must support expiration, revocation, rotation, and secure token hashing.

Do not store plaintext refresh tokens.

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
| `student_number` | VARCHAR(50) | Required within tenant |
| `first_name` | VARCHAR(100) | Required |
| `middle_name` | VARCHAR(100) | Optional |
| `last_name` | VARCHAR(100) | Required |
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
UNIQUE (tenant_id, student_number)
```

A student identity is tenant-owned, not branch-owned.

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
| `applicant_middle_name` | VARCHAR(100) | Optional |
| `applicant_last_name` | VARCHAR(100) | Required |
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

*(Awaiting continuation)*
