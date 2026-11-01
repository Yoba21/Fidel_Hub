# DATABASE ARCHITECTURE SPECIFICATION

## Fidel Hub — Persistence Design

| Document Information | Details |
| :--- | :--- |
| **Document** | Database Architecture Specification |
| **Version** | 1.0-DRAFT |
| **Database Engine** | PostgreSQL 16+ |
| **Isolation Model** | Shared Database, Row-Level Security (RLS) |

---

## 1. Multi-Tenancy Architecture
The database employs a single shared PostgreSQL database instance. Multi-tenancy is enforced natively at the database kernel level through:
1. `tenant_id UUID NOT NULL` on every tenant-scoped table.
2. PostgreSQL Row-Level Security (`ALTER TABLE ... ENABLE ROW LEVEL SECURITY`).
3. Transaction-local session configuration:
   ```sql
   SET LOCAL app.current_tenant_id = 'c4b8b6e2-...';
   ```
4. Dedicated non-superuser database role `fidel_app_user`.

---

## 2. Partitioning & Indexing Strategy
- Primary composite indexes: `(tenant_id, id)` as primary key.
- Search indexes: `(tenant_id, admission_number)`, `(tenant_id, academic_year_id)`.
- Financial indexes: `(tenant_id, transaction_date DESC)` and unique constraint on `(tenant_id, idempotency_key)`.
