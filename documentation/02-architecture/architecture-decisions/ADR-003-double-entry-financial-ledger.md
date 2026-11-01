# ADR-003: Student Fee & Payment Management (Without Full General Ledger)

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted (Revised per PRD Scope) |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
The PRD explicitly states (Section 5.12 & Out of Scope):
> *"The initial product covers fee management and does not implement a complete general ledger."*
> *"Out of Scope: Full general ledger accounting, payroll processing, full HR."*

We evaluated whether to include a full double-entry chart of accounts and journal voucher ledger, or design a streamlined **Student Fee, Invoice & Payment Tracking System**.

## 2. Decision Outcome
Chosen: **Streamlined Student Fee & Invoice Management**.

### Key Architecture & Invariants:
1. **Fee Structures & Snapshots**: Branches define fee categories (tuition, registration, transport, lab). When assigned to a student, a historical fee snapshot is preserved so subsequent fee structure edits do not retroactively alter existing student terms.
2. **Invoice Posting Lifecycle**:
   - `DRAFT` -> `POSTED` -> `PARTIALLY_PAID` -> `PAID` / `CANCELLED`.
   - Once posted, an invoice cannot be directly updated or deleted.
3. **Payments & Idempotency**:
   - Payments are recorded against posted invoices.
   - Idempotency keys (`idempotency_key` UUID) prevent duplicate charges from network retries.
   - Official sequential receipts are generated with payment reference (Cash, CBE, Telebirr, Bank Transfer).
4. **Adjustments & Waivers**:
   - Adjustments, discounts, or waivers are applied through dedicated adjustment records linked to the invoice, maintaining an auditable historical trail without requiring full double-entry accounting journals.
