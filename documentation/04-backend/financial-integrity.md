# FINANCIAL INTEGRITY & DOUBLE-ENTRY LEDGER SPECIFICATION

## Fidel Hub — Accounting Subsystem Invariants

| Document Information | Details |
| :--- | :--- |
| **Document** | Financial Integrity Specification |
| **Version** | 1.0-DRAFT |

---

## 1. Accounting Invariants
1. **Double-Entry Principle**:
   $$\sum \text{Debit} = \sum \text{Credit}$$
   Every payment, invoice fee generation, or refund creates balancing ledger lines.
2. **Immutability**:
   Once committed to the database, a journal voucher is permanent. Corrections require posting a reversal journal entry.
3. **Idempotency**:
   Every incoming payment must provide an `Idempotency-Key` (UUID). If a request with the same key is repeated, the cached receipt is returned without re-executing ledger writes.

---

## 2. Standard Transaction Scenarios

### 2.1 Tuition Invoicing
- **Debit**: Student Accounts Receivable (`1200-AR`)
- **Credit**: Tuition Fee Revenue (`4100-REV-TUIT`)

### 2.2 Parent Fee Payment (Bank / Telebirr)
- **Debit**: Cash in Bank (`1010-BNK-CBE` / `1020-TELEBIRR`)
- **Credit**: Student Accounts Receivable (`1200-AR`)

### 2.3 Fee Adjustment / Scholarship Discount
- **Debit**: Scholarship Expense (`5100-EXP-SCHOL`)
- **Credit**: Student Accounts Receivable (`1200-AR`)
