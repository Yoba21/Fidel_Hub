# BUSINESS WORKFLOWS & OPERATIONAL SPECIFICATIONS

## Fidel Hub — Standard Operating Workflows

| Document Information | Details |
| :--- | :--- |
| **Document** | Business Workflows Specification |
| **Version** | 1.0-DRAFT |
| **Status** | Template / Baseline |
| **Market** | Ethiopian K–12 Schools |

---

## 1. Student Admission & Enrollment Workflow

```mermaid
graph TD
    A[Prospective Guardian Submits Application] --> B[Registrar Reviews Application & Prerequisites]
    B -->|Incomplete/Rejected| C[Notify Guardian via SMS/Portal]
    B -->|Approved| D[Generate Unique Admission Number]
    D --> E[Assign to Academic Year, Grade Level & Section]
    E --> F[Generate Student Fee Profile & Term 1 Invoice]
    F --> G[Dispatch Welcome Pack & Parent Login Credentials]
```

### Steps:
1. **Intake**: Guardian provides student biodata, national ID / birth certificate, and previous school transfer transcripts.
2. **Review & Screening**: Admissions committee checks age eligibility and class capacity.
3. **Provisioning**: System allocates tenant-scoped admission number (e.g., `SJA-2026-0042`).
4. **Billing Activation**: Initial registration and tuition fee invoice generated automatically.

---

## 2. Daily Attendance & Absence Escalation Workflow

```mermaid
sequenceDiagram
    autonumber
    actor Teacher as Class Teacher (Mobile App)
    participant App as Mobile Local SQLite
    participant Sync as Sync Worker
    participant API as School.Api (Attendance)
    participant Comms as Communication Module (SMS)
    actor Parent as Student Parent

    Teacher->>App: Mark Daily Roster (Present / Absent / Late / Excused)
    App->>App: Store attendance batch in local SQLite
    App->>Sync: Background trigger to push batch
    Sync->>API: POST /api/v1/attendance/sessions (with Idempotency Key)
    API->>API: Validate session & commit with RLS
    API->>Comms: Publish StudentAbsenceEvent for all unexcused
    Comms->>Parent: Send Automated SMS: "[Fidel Hub] Your child was marked absent today"
```

---

## 3. Fee Billing, Payment & Double-Entry Ledger Workflow

```mermaid
sequenceDiagram
    autonumber
    actor Cashier as School Cashier / Parent
    participant API as Finance Module API
    participant Ledger as Double-Entry General Ledger
    participant Outbox as Transactional Outbox
    participant Comms as Notification Service

    Cashier->>API: Post Payment (StudentId, InvoiceId, Amount, PaymentMethod)
    API->>API: Validate invoice balance & check idempotency key
    API->>Ledger: Insert Balanced Journal Voucher (DR Cash/Bank, CR Accounts Receivable)
    API->>API: Mark invoice as Partially/Fully Paid & generate Receipt #
    API->>Outbox: Record PaymentReceivedEvent
    API-->>Cashier: Return official printable receipt with QR verification code
    Outbox->>Comms: Background job triggers receipt SMS to guardian
```

### Business Rules:
- Under no circumstances can a posted transaction be modified or deleted.
- Cancellations must be performed through an authorized `Credit Note` or `Reversal Journal Entry`.
- Partial payments are supported; outstanding balances carry forward into subsequent billing statements.

---

## 4. Examination, Continuous Assessment & Report Card Generation

```mermaid
graph TD
    A[Academic Term Setup & Weighting 30% Mid / 70% Final] --> B[Teachers Input Continuous Assessment Marks]
    B --> C[Term Examination Conducted & Scores Recorded]
    C --> D[System Aggregates Total Marks, Averages & Branch Rankings]
    D --> E[Department Head / Principal Reviews & Locks Gradebook]
    E --> F[Batch PDF Report Card Generation with Ethiopic Script]
    F --> G[Publish to Parent Portal & Send Completion SMS]
```
