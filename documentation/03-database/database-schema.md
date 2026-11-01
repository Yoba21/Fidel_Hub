# DATABASE SCHEMA SPECIFICATION

## Fidel Hub — Table Definitions & Constraints

| Document Information | Details |
| :--- | :--- |
| **Document** | Database Schema Specification |
| **Version** | 1.0-DRAFT |

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

### 1.3 `journal_entries`
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `tenant_id` | `UUID` | NOT NULL, FK to `tenants(id)` | RLS tenant isolation key |
| `id` | `UUID` | NOT NULL | Unique journal line ID |
| `voucher_number` | `VARCHAR(100)` | NOT NULL | Document voucher ID |
| `transaction_date`| `DATE` | NOT NULL | Transaction date |
| `account_code` | `VARCHAR(50)` | NOT NULL | Chart of accounts code |
| `debit_amount` | `NUMERIC(18,4)`| NOT NULL DEFAULT 0.0000 | Debit entry |
| `credit_amount` | `NUMERIC(18,4)`| NOT NULL DEFAULT 0.0000 | Credit entry |
| `memo` | `TEXT` | NULL | Description/notes |
| `idempotency_key` | `UUID` | NOT NULL, UNIQUE | Client mutation token |

**Primary Key**: `(tenant_id, id)`
