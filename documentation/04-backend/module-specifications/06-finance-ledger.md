# MODULE 06: FEE & PAYMENT MANAGEMENT SPECIFICATION

## Fidel Hub Backend Specification

| Module | Responsibility |
| :--- | :--- |
| **Domain Scope** | Fee categories, student fee assignment snapshots, term invoicing, payment collection, receipts, and balances. |

---

## 1. Domain Entities & Value Objects
- `FeeCategory`: Tuition, Registration, Lab, Transport, Uniform.
- `FeeStructure`: Grouping of fee lines for grade levels and branches.
- `StudentFeeAssignment`: Student-specific fee snapshot with approved discounts or scholarships.
- `Invoice`: Term fee invoice with status (`DRAFT`, `POSTED`, `PARTIALLY_PAID`, `PAID`, `CANCELLED`).
- `InvoiceItem`: Line item (description, quantity, unit amount, net amount).
- `Payment`: Amount, payment method, reference, idempotency token.
- `Receipt`: Sequential receipt number, cashier user, timestamp.

## 2. Key Commands & Queries
- `GenerateTermInvoicesCommand`: Batch generates invoices for enrolled students based on their fee structure snapshots.
- `PostInvoiceCommand`: Finalizes and locks invoice for payment.
- `RecordPaymentCommand`: Records payment against posted invoice with idempotency key, generates receipt.
- `GetStudentFeeStatementQuery`: Retrieves payment history, total billed, paid, and outstanding balance.
