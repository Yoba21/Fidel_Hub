# ROW-LEVEL SECURITY (RLS) & DATABASE DEFENSE

## Fidel Hub — Data Isolation Mechanics

| Document Information | Details |
| :--- | :--- |
| **Document** | RLS Security Architecture |
| **Version** | 1.0-DRAFT |

---

## 1. Security Invariants
- **Non-Superuser Execution**: Superusers bypass RLS. All app connections strictly use `fidel_app_user`.
- **Default-Deny**: In PostgreSQL, enabling RLS without an active policy defaults to denying access.
- **Tenant Context Session Scope**: `SET LOCAL` ensures tenant scoping persists only for the lifetime of that transaction.

---

## 2. RLS Policy Definition Template

```sql
-- 1. Enable RLS
ALTER TABLE class_sections ENABLE ROW LEVEL SECURITY;

-- 2. Restrictive Policy
CREATE POLICY tenant_isolation_class_sections ON class_sections
    FOR ALL
    TO fidel_app_user
    USING (tenant_id = NULLIF(current_setting('app.current_tenant_id', true), '')::uuid)
    WITH CHECK (tenant_id = NULLIF(current_setting('app.current_tenant_id', true), '')::uuid);
```
