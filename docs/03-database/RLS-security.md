# ROW-LEVEL SECURITY (RLS) & DATABASE DEFENSE SPECIFICATION

## Fidel Hub — Kernel-Level Tenant & Branch Isolation Engine

| Document Information | Details |
| :--- | :--- |
| **Document ID** | RLS-003 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Database Engine** | PostgreSQL 16+ |
| **Enforcement Layer** | Storage Kernel via PostgreSQL Row-Level Security |
| **Backend Framework** | .NET 10 LTS (`net10.0`) & EF Core 10 |
| **Prerequisites** | Developers must install .NET 10 SDK or higher. |
| **Package Dependencies** | `Microsoft.EntityFrameworkCore.*: 10.x`, `Npgsql.EntityFrameworkCore.PostgreSQL: 10.x` |
| **Container Directives** | `mcr.microsoft.com/dotnet/aspnet:10.0` |

---

## 1. Security Architecture & Threat Model

Fidel Hub stores multi-tenant school data in a shared PostgreSQL database. Security cannot rely solely on backend application filtering (`WHERE tenant_id = @id`). If a developer writes a raw query omitting the tenant filter or a zero-day injection occurs, data isolation must remain unbreakable.

```
+---------------------------------------------------------------------------------+
|                               HTTP Request Payload                              |
|                          (JWT Bearer: tenant_id = 'T1')                         |
+---------------------------------------------------------------------------------+
                                         |
                                         v
+---------------------------------------------------------------------------------+
|                       ASP.NET Core Connection Interceptor                       |
|           1. Checkout connection from Npgsql Connection Pool                    |
|           2. Execute: SET LOCAL app.current_tenant_id = 'T1';                   |
|           3. Execute: SET LOCAL app.current_branch_id = 'B1'; (if scoped)       |
+---------------------------------------------------------------------------------+
                                         |
                                         v
+---------------------------------------------------------------------------------+
|                  PostgreSQL Database Engine (fidel_app_user)                    |
|        Kernel evaluates RLS policy on EVERY SELECT, INSERT, UPDATE, DELETE:     |
|               tenant_id = NULLIF(current_setting(...), '')::uuid                |
|                                                                                 |
|      [Attempted Cross-Tenant Read on T2 Record] ----> 0 Rows Returned (Blocked) |
|      [Attempted Cross-Tenant Write to T2 Record] ---> 42501 Access Denied       |
+---------------------------------------------------------------------------------+
```

---

## 2. Core Implementation Invariants

1. **Non-Superuser Application Role (`fidel_app_user`)**:
   PostgreSQL superusers inherently bypass RLS policies (`BYPASSRLS`). The application connection pool must strictly connect under `fidel_app_user`.
2. **Transaction-Local Scoping (`SET LOCAL`)**:
   `SET LOCAL` binds the tenant context strictly to the lifetime of that database transaction, preventing connection pooling state leakage.
3. **Restrictive Policies**:
   Policies are defined `AS RESTRICTIVE` so all policies must be satisfied.
4. **Default-Deny Behavior**:
   Tables with RLS enabled and missing policies default to denying all access.

---

## 3. Comprehensive Schema RLS Policies

### 3.1 Tenant Context Resolution Functions
```sql
-- Evaluates current transaction tenant UUID
CREATE OR REPLACE FUNCTION current_tenant_id() RETURNS uuid AS $$
    SELECT NULLIF(current_setting('app.current_tenant_id', true), '')::uuid;
$$ LANGUAGE sql STABLE;

-- Evaluates current transaction branch UUID (nullable for tenant-wide access)
CREATE OR REPLACE FUNCTION current_branch_id() RETURNS uuid AS $$
    SELECT NULLIF(current_setting('app.current_branch_id', true), '')::uuid;
$$ LANGUAGE sql STABLE;
```

---

### 3.2 Student Information System (`students`)
```sql
ALTER TABLE students.students ENABLE ROW LEVEL SECURITY;
ALTER TABLE students.students FORCE ROW LEVEL SECURITY;

CREATE POLICY tenant_isolation_students ON students.students
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE students.guardians ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_guardians ON students.guardians
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());
```

---

### 3.3 Examination & CBT Schema (`examinations`)
```sql
ALTER TABLE examinations.question_banks ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_question_banks ON examinations.question_banks
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE examinations.questions ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_questions ON examinations.questions
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE examinations.exam_papers ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_exam_papers ON examinations.exam_papers
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE examinations.cbt_sessions ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_cbt_sessions ON examinations.cbt_sessions
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE examinations.student_answers ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_student_answers ON examinations.student_answers
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());
```

---

### 3.4 Finance & Invoicing Schema (`finance`)
```sql
ALTER TABLE finance.fee_structures ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_fee_structures ON finance.fee_structures
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE finance.student_fee_assignments ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_fee_assignments ON finance.student_fee_assignments
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE finance.invoices ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_invoices ON finance.invoices
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE finance.payments ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_payments ON finance.payments
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());

ALTER TABLE finance.receipts ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_receipts ON finance.receipts
    FOR ALL TO fidel_app_user
    USING (tenant_id = current_tenant_id())
    WITH CHECK (tenant_id = current_tenant_id());
```

---

### 3.5 Academic, Attendance & Health Schemas
```sql
ALTER TABLE academic.academic_years ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_academic_years ON academic.academic_years
    FOR ALL TO fidel_app_user USING (tenant_id = current_tenant_id());

ALTER TABLE academic.timetables ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_timetables ON academic.timetables
    FOR ALL TO fidel_app_user USING (tenant_id = current_tenant_id());

ALTER TABLE attendance.sessions ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_attendance_sessions ON attendance.sessions
    FOR ALL TO fidel_app_user USING (tenant_id = current_tenant_id());

ALTER TABLE attendance.records ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_attendance_records ON attendance.records
    FOR ALL TO fidel_app_user USING (tenant_id = current_tenant_id());

ALTER TABLE health.student_health_profiles ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_health_profiles ON health.student_health_profiles
    FOR ALL TO fidel_app_user USING (tenant_id = current_tenant_id());
```

---

## 4. Verification & Defense Testing

Automated integration tests must execute the following scenarios:
1. **Direct PK Probe**: Tenant A requests `SELECT * FROM students.students WHERE id = '{TenantB_StudentId}'`. Assert return is `0 rows`.
2. **Missing Tenant Setting**: A query executed without calling `SET LOCAL app.current_tenant_id` must return `0 rows` (default-deny).
3. **Cross-Tenant Injection Attempt**: Calling `INSERT INTO finance.payments (tenant_id, ...)` with Tenant B's ID while session is Tenant A must throw `PostgresException: 42501 (new row violates row-level security policy)`.
