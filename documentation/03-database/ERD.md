# ENTITY RELATIONSHIP DIAGRAM (ERD)

## Fidel Hub — Relational Schema Models

| Document Information | Details |
| :--- | :--- |
| **Document** | Entity Relationship Diagram |
| **Version** | 1.0-DRAFT |

---

## 1. High-Level Core ERD

```mermaid
erDiagram
    TENANTS ||--o{ BRANCHES : owns
    TENANTS ||--o{ USERS : registers
    TENANTS ||--o{ STUDENTS : enrolls
    TENANTS ||--o{ ACADEMIC_YEARS : defines
    TENANTS ||--o{ JOURNAL_ENTRIES : records

    BRANCHES ||--o{ CLASS_SECTIONS : contains
    CLASS_SECTIONS ||--o{ ENROLLMENTS : places
    STUDENTS ||--o{ ENROLLMENTS : has
    STUDENTS ||--o{ ATTENDANCE_RECORDS : logs
    STUDENTS ||--o{ FEE_INVOICES : billed

    FEE_INVOICES ||--o{ INVOICE_ITEMS : includes
    FEE_INVOICES ||--o{ PAYMENTS : settles
    PAYMENTS ||--|| JOURNAL_ENTRIES : creates

    TENANTS {
        uuid id PK
        varchar name
        varchar subdomain UK
        boolean is_active
    }

    STUDENTS {
        uuid tenant_id FK
        uuid id PK
        varchar admission_number
        varchar first_name
        varchar last_name
        date date_of_birth
        varchar status
    }

    JOURNAL_ENTRIES {
        uuid tenant_id FK
        uuid id PK
        varchar voucher_number
        date transaction_date
        varchar account_code
        numeric debit_amount
        numeric credit_amount
        uuid idempotency_key UK
    }
```
