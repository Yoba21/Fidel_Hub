# ADR-002: PostgreSQL Row-Level Security (RLS) for Multi-Tenancy

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
Fidel Hub is a SaaS platform hosting hundreds of independent schools. Cross-tenant data leakage is unacceptable. We evaluated:
1. Database-per-tenant (separate database for each school).
2. Schema-per-tenant (separate Postgres schema per school).
3. Shared database, shared schema with Row-Level Security (RLS).

## 2. Decision Outcome
Chosen **Option 3: Shared Database + PostgreSQL RLS + Composite Foreign Keys**.

### Implementation Rules:
1. Every tenant-scoped entity has `tenant_id uuid NOT NULL`.
2. Connection checkout sets `SET LOCAL app.current_tenant_id = '<guid>'`.
3. Application connects via non-superuser role `fidel_app_user`.
4. Foreign keys enforce composite pairs `(tenant_id, id)` preventing cross-tenant child assignment.

### Trade-offs & Mitigations:
- *Risk*: A developer writes a raw SQL query forgetting `WHERE tenant_id = @id`.
- *Mitigation*: PostgreSQL RLS operates at the database storage engine layer. Even raw queries execute under the RLS policy filter, ensuring zero cross-tenant leakage.
