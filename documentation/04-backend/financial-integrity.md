# FEE & INVOICE PAYMENT INTEGRITY SPECIFICATION

## Fidel Hub — Fee Tracking Subsystem Invariants

| Document Information | Details |
| :--- | :--- |
| **Document** | Fee & Invoice Payment Integrity Specification |
| **Version** | 2.0-ALIGNED |
| **Scope** | Student Fee Structures, Invoices, Payment Allocation & Receipts |

---

## 1. System Invariants (Per PRD Section 5.12)
1. **Branch-Specific Financial Scope**:
   Financial fee structures, invoices, and payments belong strictly to authorized branches.
2. **Fee Historical Snapshots**:
   When a student is assigned fee structures, a snapshot of the fee rates is preserved to prevent subsequent institutional fee updates from altering past or active term billings.
3. **Immutable Posted Invoices**:
   Once an invoice is transitioned to `POSTED`, direct inline modification or deletion is prohibited. Corrections must occur via explicit adjustment items or authorized cancellation.
4. **Idempotent Payment Recording**:
   Payment requests require an `Idempotency-Key` (UUID). Replaying a payment with the same key returns the existing confirmed receipt rather than charging or crediting the student twice.
5. **Sequential Official Receipts**:
   Every confirmed payment generates an immutable, sequentially numbered receipt with branch identifier, payment method (Cash, Telebirr, CBE Birr, Bank Transfer), and verification hash/QR.
