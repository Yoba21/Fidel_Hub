# DATABASE SCHEMA SPECIFICATION

## Fidel Hub — Table Definitions & Constraints

| Document Information | Details |
| :--- | :--- |
| **Document** | Database Schema Specification |
| **Version** | 2.0-ALIGNED |
| **Note** | Aligned with PRD: Fee & Invoice Tracking (No Full GL) & CBT Offline Exam Sync |

---

## 1. Table Definitions

### 1.1 `tenants`
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `id` | `UUID` | PRIMARY KEY | Unique tenant ID |
| `name` | `VARCHAR(255)` | NOT NULL | School organization name |
| `subdomain` | `VARCHAR(100)` | NOT NULL, UNIQUE | Subdomain identifier |
| `is_active` | `BOOLEAN` | NOT NULL DEFAULT true | Subscription status |
| `created_at` | `TIMESTAMPTZ` | NOT NULL DEFAULT NOW() | Timestamp |

### 1.2 `students`
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `tenant_id` | `UUID` | NOT NULL, FK to `tenants(id)` | RLS tenant isolation key |
| `id` | `UUID` | NOT NULL | Unique student ID |
| `branch_id` | `UUID` | NOT NULL | School campus/branch |
| `admission_number` | `VARCHAR(50)` | NOT NULL | Unique per tenant |
| `first_name` | `VARCHAR(100)` | NOT NULL | Student first name |
| `father_name` | `VARCHAR(100)` | NOT NULL | Student father name |
| `grandfather_name`| `VARCHAR(100)` | NOT NULL | Student grandfather name |
| `date_of_birth` | `DATE` | NOT NULL | Birth date |
| `gender` | `VARCHAR(10)` | NOT NULL | Male / Female |
| `created_at` | `TIMESTAMPTZ` | NOT NULL DEFAULT NOW() | Record creation time |

**Primary Key**: `(tenant_id, id)`
**Unique Constraint**: `(tenant_id, admission_number)`

### 1.3 `invoices`
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `tenant_id` | `UUID` | NOT NULL, FK to `tenants(id)` | RLS tenant isolation key |
| `id` | `UUID` | NOT NULL | Unique invoice ID |
| `branch_id` | `UUID` | NOT NULL | School branch ID |
| `student_id` | `UUID` | NOT NULL | Student ID |
| `invoice_number` | `VARCHAR(50)` | NOT NULL | Unique per tenant invoice number |
| `academic_term_id`| `UUID` | NOT NULL | Term reference |
| `total_amount` | `NUMERIC(14,2)`| NOT NULL | Total billed amount |
| `paid_amount` | `NUMERIC(14,2)`| NOT NULL DEFAULT 0.00 | Amount paid so far |
| `status` | `VARCHAR(20)` | NOT NULL DEFAULT 'DRAFT' | DRAFT, POSTED, PARTIAL, PAID, CANCELLED |
| `due_date` | `DATE` | NOT NULL | Invoice payment deadline |
| `created_at` | `TIMESTAMPTZ` | NOT NULL DEFAULT NOW() | Creation timestamp |

**Primary Key**: `(tenant_id, id)`

### 1.4 `payments`
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `tenant_id` | `UUID` | NOT NULL, FK to `tenants(id)` | RLS tenant isolation key |
| `id` | `UUID` | NOT NULL | Unique payment record ID |
| `branch_id` | `UUID` | NOT NULL | Branch ID |
| `invoice_id` | `UUID` | NOT NULL | Target invoice |
| `amount` | `NUMERIC(14,2)`| NOT NULL | Payment amount |
| `payment_method` | `VARCHAR(30)` | NOT NULL | CASH, CBE_BIRR, TELEBIRR, BANK_SLIP |
| `reference_number`| `VARCHAR(100)` | NULL | Bank / Gateway confirmation ref |
| `idempotency_key` | `UUID` | NOT NULL, UNIQUE | Mutation token preventing duplicates |
| `created_at` | `TIMESTAMPTZ` | NOT NULL DEFAULT NOW() | Payment timestamp |

**Primary Key**: `(tenant_id, id)`

### 1.5 `cbt_exam_sessions` (Offline CBT Engine)
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `tenant_id` | `UUID` | NOT NULL, FK to `tenants(id)` | RLS tenant isolation key |
| `id` | `UUID` | NOT NULL | Server session ID |
| `exam_id` | `UUID` | NOT NULL | Exam identifier |
| `student_id` | `UUID` | NOT NULL | Student ID |
| `client_sync_id` | `UUID` | NOT NULL, UNIQUE | Client UUID generated offline |
| `started_at` | `TIMESTAMPTZ` | NOT NULL | Exam start time |
| `submitted_at` | `TIMESTAMPTZ` | NOT NULL | Exam completion time |
| `answers_json` | `JSONB` | NOT NULL | Submitted answers payload |
| `score` | `NUMERIC(6,2)` | NULL | Auto-graded result |
| `sync_status` | `VARCHAR(20)` | NOT NULL DEFAULT 'SYNCED' | Sync status |

**Primary Key**: `(tenant_id, id)`
