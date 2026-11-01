# SECURITY TEST PLAN & PENETRATION TESTING RUNBOOK

## Fidel Hub — Enterprise Application Security & Multi-Tenant Isolation Testing

| Document Information | Details |
| :--- | :--- |
| **Document ID** | SEC-001 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Scope** | Kernel RLS Penetration, Better Auth JWT Validation, STRIDE Threat Model |
| **Compliance** | Ethiopian Personal Data Protection Standards & OWASP ASVS 4.0 |

---

## 1. STRIDE Threat Model & Security Controls

| Threat Category | Primary Attack Vector | Fidel Hub Defensive Control | Verification Test Case |
| :--- | :--- | :--- | :--- |
| **Spoofing** | Forging JWT bearer tokens or faking student exam logins | Better Auth cryptographic signatures (RS256/EdDSA) verified against JWKS | Test SEC-001 (Forged Token Rejection) |
| **Tampering** | Intercepting CBT answers or modifying locked gradebooks | AES-256 encrypted exam bundles + append-only audit trail | Test SEC-002 (Gradebook Lock Tampering) |
| **Repudiation** | Cashier denying cash receipt or teacher denying mark edits | Sequential receipts + immutable audit logs with staff ID & IP | Test SEC-003 (Audit Trail Immutability) |
| **Information Disclosure** | Cross-tenant data leakage via SQL query parameter manipulation | PostgreSQL Kernel Row-Level Security (RLS) on all 15 schemas | Test SEC-004 (Kernel RLS Boundary Probe) |
| **Denial of Service** | Flooding CBT Venue Server during active exams | SQLite WAL mode (sub-50ms writes) + Rate limiting with Redis | Test SEC-005 (Autosave Stress Benchmark) |
| **Elevation of Privilege** | Teacher attempting to perform principal or cashier operations | ASP.NET Core Policy Handlers validating assigned permissions | Test SEC-006 (Role Capability Enforcement) |

---

## 2. PostgreSQL Kernel RLS Penetration Testing

The primary security invariant of Fidel Hub is that **no tenant may read or mutate data belonging to another tenant, even if the application code contains an accidental flaw or omit-where bug**:

### 2.1 RLS Penetration Test Execution Script
```sql
-- Security Test Case: SEC-RLS-01 (Direct RLS Bypass Attempt)
BEGIN;

-- 1. Assume the unprivileged application role
SET ROLE fidel_app_user;

-- 2. Inject Tenant A context into session
SET LOCAL app.current_tenant_id = '11111111-1111-1111-1111-111111111111';

-- 3. Attempt direct SELECT targeting Tenant B's student UUID
SELECT id, first_name, tenant_id FROM students.students 
WHERE id = '99999999-9999-9999-9999-999999999999';
-- Expected Result: Zero rows returned (PostgreSQL kernel drops row silently)

-- 4. Attempt direct INSERT of a student record belonging to Tenant B
INSERT INTO students.students (id, tenant_id, branch_id, admission_number, first_name, father_name, grandfather_name, date_of_birth, gender, status, created_at, created_by)
VALUES (gen_random_uuid(), '22222222-2222-2222-2222-222222222222', '33333333-3333-3333-3333-333333333333', 'HACK-001', 'Evil', 'Intruder', 'Attacker', '2010-01-01', 'Male', 'ACTIVE', NOW(), gen_random_uuid());
-- Expected Result: ERROR: new row violates row-level security policy for table "students"

ROLLBACK;
```

---

## 3. Better Auth & Token Security Test Matrix

### 3.1 Algorithm Confusion Attack (`alg: none` / `alg: HS256`)
* **Test**: Submit a request with a JWT where the header specifies `"alg": "none"` or swaps the public RS256 key into an HMAC symmetric secret.
* **Pass Criterion**: ASP.NET Core `JwtBearer` rejects the token with HTTP 401 Unauthorized before reaching any controller action.

### 3.2 Branch Hopping Attack
* **Scenario**: A teacher assigned exclusively to "Bole Campus" manually changes the `branch_id` header or body parameter to target "Piassa Campus".
* **Pass Criterion**: The `TenantBranchResolutionMiddleware` checks `identity.user_branch_access`, recognizes the user lacks permission for Piassa Campus, and terminates the request with HTTP 403 Forbidden.

---

## 4. Student Data Privacy & Minor Protection

In accordance with Ethiopian data protection legislation and international child safety standards:
1. **PII Masking**: Guardian national ID numbers, medical allergy files, and student clinic visits are masked in standard administrative logs.
2. **Encrypted Media Storage**: Birth certificates and transfer documents uploaded during admissions intake are stored in private S3 buckets using AES-256 server-side encryption; URLs expire after 15 minutes.
3. **Audit Trail Retention**: All security-critical events (grade modifications, student transfers, fee waivers) are retained in `audit.audit_logs` for a minimum of 7 academic years.
