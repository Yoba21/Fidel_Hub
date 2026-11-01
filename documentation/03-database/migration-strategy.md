# DATABASE MIGRATION STRATEGY

## Fidel Hub — Schema Lifecycle Management

| Document Information | Details |
| :--- | :--- |
| **Document** | Database Migration Strategy |
| **Version** | 1.0-DRAFT |

---

## 1. Migration Pipeline
- Schema migrations are authored using EF Core migrations per module:
  `dotnet ef migrations add <Name> --project Modules/<Module>`
- Production execution: Migrations are idempotent and run automatically during deployment pipelines or through a dedicated migration runner container.

---

## 2. Zero-Downtime Migration Rules
- Never drop a column or rename it in a single release.
- Follow the **Expand / Contract** pattern:
  1. Release 1: Add new nullable column.
  2. Release 2: Dual-write to both columns.
  3. Release 3: Backfill data and switch reads to new column.
  4. Release 4: Remove old column.
