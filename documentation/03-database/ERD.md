# ENTITY RELATIONSHIP DIAGRAM (ERD)

## Fidel Hub — Relational Schema Models

| Document Information | Details |
| :--- | :--- |
| **Document** | Entity Relationship Diagram |
| **Version** | 2.0-ALIGNED |
| **Scope** | Core Multi-Tenant School Operations, Fees & CBT Examinations |

---

## 1. High-Level Core ERD

```mermaid
erDiagram
    TENANTS ||--o{ BRANCHES : owns
    TENANTS ||--o{ USERS : registers
    TENANTS ||--o{ STUDENTS : enrolls
    TENANTS ||--o{ ACADEMIC_YEARS : defines

    BRANCHES ||--o{ CLASS_SECTIONS : contains
    CLASS_SECTIONS ||--o{ ENROLLMENTS : places
    STUDENTS ||--o{ ENROLLMENTS : has
    STUDENTS ||--o{ INVOICES : billed

    INVOICES ||--o{ INVOICE_ITEMS : details
    INVOICES ||--o{ PAYMENTS : receives
    PAYMENTS ||--|| RECEIPTS : issues

    TENANTS ||--o{ CBT_EXAMS : creates
    CBT_EXAMS ||--o{ CBT_QUESTIONS : contains
    CBT_EXAMS ||--o{ CBT_SESSIONS : delivers
    STUDENTS ||--o{ CBT_SESSIONS : takes
    CBT_SESSIONS ||--o{ CBT_ANSWERS : records

    TENANTS {
        uuid id PK
        varchar name
        varchar subdomain UK
        boolean is_active
    }

    STUDENTS {
        uuid tenant_id FK
        uuid id PK
        uuid branch_id FK
        varchar admission_number UK
        varchar first_name
        varchar last_name
        varchar status
    }

    INVOICES {
        uuid tenant_id FK
        uuid id PK
        uuid student_id FK
        varchar invoice_number UK
        numeric total_amount
        numeric paid_amount
        varchar status
    }

    PAYMENTS {
        uuid tenant_id FK
        uuid id PK
        uuid invoice_id FK
        numeric amount
        varchar payment_method
        varchar reference_number
        uuid idempotency_key UK
    }

    CBT_SESSIONS {
        uuid tenant_id FK
        uuid id PK
        uuid exam_id FK
        uuid student_id FK
        timestamp started_at
        timestamp completed_at
        varchar status
        uuid client_sync_id UK
    }
```
