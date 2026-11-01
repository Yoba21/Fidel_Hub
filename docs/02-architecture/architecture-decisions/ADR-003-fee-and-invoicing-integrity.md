# ADR-003: Student Fee & Payment Management (Without Full General Ledger)

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted (Per PRD Section 5.12 & Out of Scope Directives) |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
The PRD states:
> *"The initial product covers fee management and does not implement a complete general ledger."*
> *"Out of Scope: Full general ledger accounting, payroll processing, full HR."*

We evaluated whether to construct a double-entry general ledger with chart of accounts or design a focused **Student Fee, Invoicing & Payment Tracking System**.

## 2. Decision Outcome
Chosen: **Student Fee & Invoicing System with Historical Snapshots & Idempotency**.

### Principles:
1. **Fee Snapshots**: Captures student-specific fee assignment snapshots so subsequent branch fee changes do not retroactively modify past or active term obligations.
2. **Invoice State Machine**: `DRAFT` → `POSTED` → `PARTIALLY_PAID` → `PAID` / `CANCELLED`.
3. **Immutability**: Posted invoices cannot be modified in place. Adjustments require explicit credit waivers.
4. **Idempotency Protection**: Incoming payment mutations require an `Idempotency-Key` (UUID), preventing duplicate charges on unstable connections.
5. **Sequential Receipts**: Official sequential receipts generated with unique verification tokens.
