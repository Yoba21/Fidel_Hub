# ADR-002: PostgreSQL Row-Level Security (RLS) for Multi-Tenancy

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
Fidel Hub is a SaaS platform hosting hundreds of independent schools. Cross-tenant data leakage is catastrophic. We evaluated database-per-tenant, schema-per-tenant, and shared-database with Row-Level Security (RLS).

## 2. Decision Outcome
Chosen: **Shared Database with PostgreSQL Kernel-Level Row-Level Security (RLS) & Composite Foreign Keys**.

### Invariants:
1. Every tenant-scoped entity has `tenant_id UUID NOT NULL`.
2. Connection checkout executes `SET LOCAL app.current_tenant_id = '<current_tenant_uuid>'`.
3. Application pool connects exclusively via the non-superuser role `fidel_app_user`.
4. Kernel-level policy: `USING (tenant_id = NULLIF(current_setting('app.current_tenant_id', true), '')::uuid)`.
5. Standardized Primary & Foreign Keys: Tables use single-column UUID primary keys (`PRIMARY KEY (id)`) paired with secondary composite unique constraints (`UNIQUE (tenant_id, id)`) and composite foreign keys (`FOREIGN KEY (tenant_id, foreign_id) REFERENCES target_table (tenant_id, id)`), preventing cross-tenant referencing at the storage constraint level while maintaining standard ORM identity mapping.
