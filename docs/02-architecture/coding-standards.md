# CODING STANDARDS & ENGINEERING GUIDELINES

## Fidel Hub — Engineering Quality Standards

| Document Information | Details |
| :--- | :--- |
| **Document ID** | STD-001 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |

---

## 1. General Engineering Principles
- **Explicit Domain Rules**: Business invariants belong in rich domain entities with private setters, not anemic data models with public properties.
- **Fail Fast & Explicitly**: Use `Result<T>` rather than throwing business exceptions for anticipated validation or domain rejections.
- **Purely Asynchronous I/O**: All database, HTTP, and disk I/O operations must be fully asynchronous (`async/await`) with `CancellationToken` cascading.

---

## 2. C# / .NET Standards
- **Language Level**: C# 12 / .NET 8 / 10 LTS.
- **Nullable Reference Types**: Mandatory `<Nullable>enable</Nullable>`. Code must compile with zero unhandled null warnings.
- **Architecture Enforcement**: Automated NetArchTest rules verify that domain entities do not depend on external infrastructure or presentation layers.
- **Database Access**: EF Core for command mutations and relationship tracking; Dapper for high-volume paginated read queries and complex reports.

---

## 3. TypeScript & React Standards
- **Strict Typing**: No `any` or loose types. All props, API requests, and response payloads must have explicit TypeScript interfaces.
- **Design Tokens**: Never hardcode raw color hex strings in components. Reference `theme.palette.primary.main`, `theme.palette.secondary.main`, or CSS custom properties.
- **Data Tables**: Table views must provide empty state fallbacks, loading skeletons, and localized dates (supporting both Gregorian and Ethiopian formats).
- **SQLite WAL**: Mobile and local venue server operations must configure `PRAGMA journal_mode = WAL;` and wrap bulk question/answer insertions in local transactions.
