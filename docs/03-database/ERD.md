# ENTITY RELATIONSHIP DIAGRAM (ERD)

## Fidel Hub — Relational Schema Models

| Document Information | Details |
| :--- | :--- |
| **Document ID** | ERD-002 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Target Database** | PostgreSQL 16+ (Multi-Tenant with Row-Level Security) |

---

## 1. High-Level Domain Relationships

```mermaid
erDiagram
    TENANTS ||--o{ BRANCHES : contains
    TENANTS ||--o{ USERS : registers
    TENANTS ||--o{ STUDENTS : enrolls
    TENANTS ||--o{ QUESTION_BANKS : owns
    
    BRANCHES ||--o{ ACADEMIC_YEARS : schedules
    BRANCHES ||--o{ CLASSES : structures
    BRANCHES ||--o{ FEE_STRUCTURES : configures
    BRANCHES ||--o{ EXAM_PAPERS : organizes
    
    CLASSES ||--o{ SECTIONS : divides
    SECTIONS ||--o{ ENROLLMENTS : places
    STUDENTS ||--o{ ENROLLMENTS : registers
    STUDENTS ||--o{ INVOICES : billed
    STUDENTS ||--o{ CBT_SESSIONS : sits
    
    QUESTION_BANKS ||--o{ QUESTIONS : contains
    EXAM_PAPERS ||--o{ CBT_SESSIONS : delivers
    CBT_SESSIONS ||--o{ STUDENT_ANSWERS : records
    
    INVOICES ||--o{ INVOICE_ITEMS : details
    INVOICES ||--o{ PAYMENTS : receives
    PAYMENTS ||--|| RECEIPTS : generates
```

---

## 2. Examination, Question Bank & CBT Data Model

```mermaid
erDiagram
    QUESTION_BANKS ||--o{ QUESTIONS : organizes
    QUESTIONS ||--o{ QUESTION_OPTIONS : defines
    
    EXAM_PAPERS ||--o{ EXAM_SCHEDULES : timetables
    EXAM_SCHEDULES ||--o{ CBT_SESSIONS : instances
    STUDENTS ||--o{ CBT_SESSIONS : takes
    CBT_SESSIONS ||--o{ STUDENT_ANSWERS : answers
    QUESTIONS ||--o{ STUDENT_ANSWERS : responds_to
    
    QUESTION_BANKS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        varchar name
        uuid subject_id FK
        uuid class_id FK
    }

    QUESTIONS {
        uuid id PK
        uuid tenant_id FK
        uuid question_bank_id FK
        text question_text
        varchar question_type
        varchar difficulty
        varchar cognitive_level
        numeric points
        text media_url
    }

    QUESTION_OPTIONS {
        uuid id PK
        uuid tenant_id FK
        uuid question_id FK
        text option_text
        int option_order
        boolean is_correct
    }

    EXAM_PAPERS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        varchar title
        numeric total_marks
        int duration_minutes
        boolean shuffle_questions
    }

    CBT_SESSIONS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid exam_paper_id FK
        uuid student_id FK
        uuid session_token UK
        timestamp started_at
        timestamp submitted_at
        int time_spent_seconds
        boolean is_final
        varchar status
        numeric total_score
    }

    STUDENT_ANSWERS {
        uuid id PK
        uuid tenant_id FK
        uuid session_id FK
        uuid question_id FK
        jsonb selected_option_ids
        text text_answer
        numeric score_awarded
        boolean is_auto_graded
        int autosave_version
    }
```

---

## 3. Student SIS, Admissions & Health Model

```mermaid
erDiagram
    STUDENTS ||--o{ STUDENT_GUARDIANS : links
    GUARDIANS ||--o{ STUDENT_GUARDIANS : associates
    STUDENTS ||--o{ APPLICATIONS : promoted_from
    STUDENTS ||--|| HEALTH_PROFILES : has
    STUDENTS ||--o{ CLINIC_VISITS : logs
    
    STUDENTS {
        uuid id PK
        uuid tenant_id FK
        varchar student_number UK
        varchar first_name
        varchar father_name
        varchar grandfather_name
        date date_of_birth
        varchar status
    }

    GUARDIANS {
        uuid id PK
        uuid tenant_id FK
        varchar first_name
        varchar last_name
        varchar phone
        varchar email
    }

    STUDENT_GUARDIANS {
        uuid tenant_id FK
        uuid student_id FK
        uuid guardian_id FK
        varchar relationship
        boolean is_primary
        boolean is_emergency_contact
        boolean can_collect_student
    }

    HEALTH_PROFILES {
        uuid id PK
        uuid tenant_id FK
        uuid student_id FK
        varchar blood_group
        text allergies
        text emergency_instructions
    }

    CLINIC_VISITS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid student_id FK
        timestamp visit_date
        text symptoms
        text action_taken
    }
```

---

## 4. Fee & Invoicing Relational Model (No General Ledger)

```mermaid
erDiagram
    FEE_STRUCTURES ||--o{ FEE_STRUCTURE_LINES : specifies
    FEE_CATEGORIES ||--o{ FEE_STRUCTURE_LINES : categorizes
    FEE_STRUCTURES ||--o{ STUDENT_FEE_ASSIGNMENTS : assigned_via
    STUDENTS ||--o{ STUDENT_FEE_ASSIGNMENTS : contracted
    
    STUDENTS ||--o{ INVOICES : billed_to
    INVOICES ||--o{ INVOICE_ITEMS : contains
    INVOICES ||--o{ PAYMENTS : credited_by
    PAYMENTS ||--|| RECEIPTS : documents

    FEE_CATEGORIES {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        varchar name
        varchar code UK
        boolean is_recurring
    }

    FEE_STRUCTURES {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid class_id FK
        varchar name
        numeric total_base_amount
    }

    STUDENT_FEE_ASSIGNMENTS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid student_id FK
        uuid fee_structure_id FK
        numeric discount_percentage
        numeric custom_net_amount
    }

    INVOICES {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid student_id FK
        varchar invoice_number UK
        numeric total_amount
        numeric paid_amount
        varchar status
        date due_date
    }

    INVOICE_ITEMS {
        uuid id PK
        uuid tenant_id FK
        uuid invoice_id FK
        uuid fee_category_id FK
        varchar description
        numeric unit_price
        numeric line_total
    }

    PAYMENTS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid invoice_id FK
        numeric amount
        varchar payment_method
        varchar reference_number
        uuid idempotency_key UK
    }

    RECEIPTS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid payment_id FK
        varchar receipt_number UK
        timestamp issued_at
    }
```

---

## 5. Academics, Timetable & Behavioral Traits Model

```mermaid
erDiagram
    CLASSES ||--o{ SECTIONS : subdivides
    SECTIONS ||--o{ TIMETABLE_SLOTS : schedules
    SUBJECTS ||--o{ TIMETABLE_SLOTS : taught_in
    TEACHERS ||--o{ TIMETABLE_SLOTS : instructs
    
    LESSON_PLANS }o--|| SUBJECTS : plans_for
    LESSON_PLANS }o--|| TEACHERS : authored_by
    
    STUDENTS ||--o{ TRAIT_SCORES : evaluated
    AFFECTIVE_TRAITS ||--o{ TRAIT_SCORES : criteria

    TIMETABLE_SLOTS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid section_id FK
        int day_of_week
        int period_number
        time start_time
        time end_time
        uuid subject_id FK
        uuid teacher_id FK
        varchar room_number
    }

    LESSON_PLANS {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid staff_member_id FK
        int week_number
        varchar topic_title
        text learning_objectives
        varchar status
    }

    AFFECTIVE_TRAITS {
        uuid id PK
        uuid tenant_id FK
        varchar trait_name
        varchar trait_category
    }

    TRAIT_SCORES {
        uuid id PK
        uuid tenant_id FK
        uuid branch_id FK
        uuid student_id FK
        uuid trait_id FK
        int rating_score
    }
```
