# ADR-003: Double-Entry General Ledger for Financial Integrity

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
School fee transactions, tuition invoices, discounts, and payments must withstand strict accounting audits. Simple "status = Paid" flags on invoices frequently lead to unreconciled bank deposits, accidental deletions, or discrepancies.

## 2. Decision Outcome
Chosen **Double-Entry General Ledger Pattern**.

### Invariants:
1. Every financial transaction generates a balanced journal voucher: $\sum \text{Debit} = \sum \text{Credit}$.
2. Records are immutable and append-only. No `DELETE` or in-place `UPDATE` of financial amounts.
3. Errors are reversed via compensating credit/debit vouchers.
4. Idempotency keys prevent double charge on flaky internet connections.
