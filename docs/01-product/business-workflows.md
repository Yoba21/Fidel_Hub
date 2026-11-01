# BUSINESS WORKFLOWS & OPERATIONAL SPECIFICATIONS

## Fidel Hub — Standard Operating Procedures & Lifecycle Workflows

| Document Information | Details |
| :--- | :--- |
| **Document ID** | BWF-001 |
| **Version** | 1.1 (Comprehensive Institutional Lifecycle Workflows) |
| **Status** | Approved Baseline |
| **Scope** | End-to-End Operational Lifecycle Across All 15 Modules |

---

## 1. Student Admission & Enrollment Lifecycle

```mermaid
sequenceDiagram
    autonumber
    actor Parent as Prospective Guardian
    participant Portal as Admissions Web Portal
    participant API as SIS & Admissions Module
    participant DB as PostgreSQL (RLS)
    participant Fee as Fee & Invoicing Module
    participant Comms as Notification Service

    Parent->>Portal: Submit Online Application (Biodata, Previous School Records, Documents)
    Portal->>API: POST /api/v1/admissions/applications
    API->>DB: Insert Application (Status: 'SUBMITTED')
    API-->>Parent: Application Acknowledgment & Reference # (e.g. APP-2026-012)

    Note over API,DB: Registrar Review & Screening
    actor Registrar as School Registrar
    Registrar->>Portal: Review Documents, Health Notes & Verify Age Criteria
    Registrar->>Portal: Approve Admission Application
    Portal->>API: POST /api/v1/admissions/applications/{id}/approve
    API->>DB: Generate Unique Tenant Admission Number (e.g. FHA-2026-0042)
    API->>DB: Insert Student Profile, Guardians & Medical Notes
    API->>DB: Insert Academic Enrollment (Grade 9, Section B)
    
    Note over API,Fee: Automated Initial Fee Setup
    API->>Fee: Create Student Fee Assignment Snapshot
    Fee->>DB: Generate Term 1 Fee Invoice (Registration + Tuition)
    
    API->>Comms: Enqueue Welcome & Admission Event
    Comms->>Parent: Send SMS: "Admission Approved! Student ID: FHA-2026-0042. Login credentials created."
```

### Operational Invariants:
1. **Three-Part Ethiopian Naming**: Admission records strictly record First Name, Father's Name, and Grandfather's Name.
2. **Historical Enrollment Record**: Student transfers between branches do not delete the historical enrollment; a new enrollment row is inserted referencing the previous enrollment ID.
3. **Fee Rate Immutability**: The initial fee assignment captures an immutable snapshot of approved discount rates and tuition tiers.

---

## 2. Academic Curriculum, Timetable & Lesson Plan Workflow

```mermaid
graph TD
    A[Academic Director Configures Academic Year & Dual Calendar] --> B[Define Grade Levels, Streams & Classroom Capacities]
    B --> C[Configure Subjects, Credit Hours & Departments]
    C --> D[Assign Teachers to Subject-Class-Section Tuples]
    D --> E[Build Weekly Timetable Schedule Grid]
    E --> F{Conflict Detection Check}
    F -->|Conflict Detected| E
    F -->|Zero Conflicts| G[Publish Class Timetable to Teachers & Students]
    G --> H[Teachers Author Weekly Lesson Plans & Syllabus Objectives]
    H --> I[Head of Department Reviews, Comments & Formally Approves]
```

### Operational Invariants:
1. **Conflict Engine**: Timetable scheduling algorithms prevent double-booking a teacher or a physical room within the same period slot.
2. **Syllabus Tracking**: Teachers submit lesson plans by Friday 5:00 PM; HOD must approve before lesson execution on Monday.

---

## 3. Daily Attendance Tracking & Truancy Escalation

```mermaid
sequenceDiagram
    autonumber
    actor Teacher as Homeroom / Subject Teacher
    participant Web as Web / Tablet Teacher Portal
    participant API as Attendance Module
    participant DB as PostgreSQL (RLS)
    participant Outbox as Transactional Outbox
    participant Worker as Background Worker Engine
    actor Parent as Student Guardian

    Teacher->>Web: Open Morning Attendance Roster for Section 8-A
    Web->>Teacher: Pre-populate Roster (Default: All Present)
    Teacher->>Web: Toggle Exceptions: Student #12 (Unexcused Absent), Student #24 (Late)
    Teacher->>Web: Submit Attendance Session
    Web->>API: POST /api/v1/attendance/sessions (Idempotency-Key)
    API->>DB: Lock class roster & insert attendance session records
    API->>Outbox: Insert OutboxMessage('StudentAbsenceEvent', StudentId, 'UNEXCUSED')
    DB-->>API: Transaction Committed
    API-->>Web: Session Saved & Locked
    
    Note over Outbox,Worker: Asynchronous Notification Dispatch
    Worker->>Outbox: Poll unpublished events
    Worker->>Worker: Resolve Primary Guardian Phone Number
    Worker->>Parent: Dispatch SMS: "[Fidel Hub] Your child Dawit Bekele was marked ABSENT today at 8:15 AM."
    Worker->>Outbox: Mark OutboxMessage as 'PROCESSED'
```

---

## 4. Computer-Based Testing (CBT) & Offline Examination Lifecycle
*(The Flagship Resilient Assessment Engine)*

```mermaid
sequenceDiagram
    autonumber
    actor Coord as Exam Coordinator
    participant API as Fidel Hub Central Cloud API
    participant Venue as Local Exam Venue Server (School LAN)
    actor Student as Student Candidate (Workstation)
    participant LocalDB as Local SQLite (WAL Mode)

    Note over Coord,API: Stage 1: Paper Creation & Scheduling
    Coord->>API: Assemble Exam Paper from Question Bank (MCQ, Formulas, Theory)
    Coord->>API: Schedule Exam: Grade 10 Biology (Start: 09:00, Duration: 60 mins)
    
    Note over API,Venue: Stage 2: Encrypted Pre-Fetch
    API->>API: Compile AES-256-GCM Encrypted Package + SHA-256 Checksum
    Venue->>API: GET /api/v1/cbt/exams/{id}/package
    API-->>Venue: Deliver Encrypted Bundle
    Venue->>Venue: Verify SHA-256 Checksum (Reject if mismatched)
    Venue->>Venue: Store encrypted bundle on disk (Time-Locked until 09:00)

    Note over Venue,Student: Stage 3: Offline Exam Sitting (100% Zero Internet)
    Venue->>Venue: System Clock Reaches 09:00 -> Decrypt Exam Paper into Local SQLite
    Student->>Venue: Authenticate on Local School Wi-Fi (http://192.168.1.50:4000)
    Venue->>LocalDB: Initialize student_exam_session (Session UUID)
    
    loop Real-Time Heartbeat & Autosave
        Student->>Venue: Select Option / Enter Response
        Venue->>LocalDB: High-Concurrency SQLite WAL Write (is_final = false, elapsed_seconds)
    end

    Note over Student,Venue: Stage 4: Cut-Off & Final Lock
    Student->>Venue: Click Submit (or 60-Minute Timer Reaches 00:00)
    Venue->>LocalDB: Hard-Lock Session: is_final = true, submitted_at = now()
    
    Note over Venue,API: Stage 5: Cloud Sync & Ingestion
    Venue->>Venue: Internet Connectivity Restored Post-Exam
    Venue->>API: POST /api/v1/cbt/sessions/sync (Batched Sessions + Answers)
    API->>API: Validate signatures, Auto-Grade Objective MCQs
    API->>API: Upsert Records ON CONFLICT (session_id, question_id) - Zero Duplication
    API-->>Venue: 200 OK (Sync Acknowledged)
```

---

## 5. Continuous Assessment, Gradebook & Dual-Script Report Card Workflow

```mermaid
graph TD
    A[Configure Term CA Components: 15% Quizzes + 35% Midterm + 50% Exam] --> B[Teachers Enter Class Scores in Spreadsheet Gradebook]
    B --> C[CBT Exam Objective Scores Auto-Populated from Assessment Engine]
    C --> D[Teachers Evaluate Subjective Theory Questions using Rubrics]
    D --> E[Affective & Psychomotor Behavioral Traits Scored 1-5 Scale]
    E --> F[System Aggregates Total Weighted Marks, GPA & Percentiles]
    F --> G[Automatic Computation of Class Rank & Branch Promotion Recommendations]
    G --> H[Principal & Head of Department Review and Lock Gradebook]
    H --> I[Batch PDF Generation: Dual-Script English + Ge'ez ፊ with Verification QR]
    I --> J[Publish to Parent Portal & Send Completion Notification SMS]
```

### Report Card Quality Standards:
- Official school branch letterhead and high-resolution logo.
- Multi-column performance table: Subject, Continuous Assessment Total, Final Exam Mark, Term Total, Class Average, and Subject Teacher Remarks.
- Behavioral Traits matrix rating punctuality, neatness, teamwork, and leadership.
- Cryptographic verification QR code encoding `https://verify.fidelhub.com/transcripts/{hash}`.

---

## 6. Student Fee Management, Invoicing & Payment Workflow
*(Strictly Enforcing PRD Scope — No General Ledger Bloat)*

```mermaid
sequenceDiagram
    autonumber
    actor Admin as Finance Officer / Cashier
    participant API as Fee & Invoicing Module
    participant DB as PostgreSQL (RLS)
    participant Comms as Notification Service
    actor Parent as Parent / Guardian

    Note over Admin,API: Stage 1: Setup & Invoicing
    Admin->>API: Configure Branch Fee Structure (Tuition, Lab, Uniform)
    Admin->>API: Assign Fees to Students (Capture Historical Snapshot & Approved Scholarships)
    Admin->>API: POST /api/v1/finance/invoices/generate (Batch Term 1)
    API->>DB: Insert Invoices (Status: 'DRAFT')
    Admin->>API: POST /api/v1/finance/invoices/post
    API->>DB: Lock Invoices (Status: 'POSTED', Immutability Enforced)

    Note over Parent,Admin: Stage 2: Payment Collection
    Parent->>Admin: Present Payment (Cash / Telebirr Confirmation / Bank Deposit Slip)
    Admin->>API: POST /api/v1/finance/payments<br/>(Header: Idempotency-Key: UUID)<br/>(Body: InvoiceId, Amount: ETB 4,500, Channel: 'TELEBIRR', Ref: 'TLB-94821')
    
    API->>DB: Verify Idempotency-Key & Lock Invoice Row
    API->>DB: Insert Payment Record
    API->>DB: Update Invoice paid_amount and transition status ('PAID')
    API->>DB: Generate Sequential Official Receipt (e.g. RCP-2026-00452)
    DB-->>API: Transaction Committed
    API-->>Admin: 201 Created (Printable Receipt Payload)
    Admin-->>Parent: Hand over Printed Official Receipt
    
    API->>Comms: Trigger Payment Received Event
    Comms->>Parent: Send SMS: "[Fidel Hub] Receipt #00452 confirmed. ETB 4,500 received for Dawit Bekele. Balance: ETB 0.00."
```

### Financial Business Rules:
1. **Posted Invoice Immutability**: Under no circumstances can a `POSTED` invoice be edited in place. Any reduction must occur via an authorized credit waiver record.
2. **Idempotency Protection**: Flaky internet retry submissions presenting the same `Idempotency-Key` return the existing receipt record without creating duplicate ledger credits.
3. **Branch Financial Boundary**: All fee structures, invoices, and payments belong strictly to authorized branches.

---

## 7. Library Book Lending, Circulation & Overdue Fine Settlement

```mermaid
sequenceDiagram
    autonumber
    actor Student as Student / Borrower
    actor Lib as Librarian
    participant Web as Library Desk UI
    participant API as Library Module
    participant DB as PostgreSQL (RLS)
    participant Fee as Fee Module

    Student->>Lib: Present Book & Student ID Card
    Lib->>Web: Scan Student ID Barcode & Book Barcode
    Web->>API: POST /api/v1/library/loans/checkout<br/>(student_id, book_id, due_days: 14)
    API->>DB: Check student active loan limit & book available_copies > 0
    API->>DB: Decrement book available_copies, Insert book_loan (due_date)
    DB-->>API: Loan Confirmed
    API-->>Web: Loan Created. Due Date: 15 Nov 2026

    Note over Student,Lib: Return Workflow & Fine Check
    Student->>Lib: Return Book on 20 Nov 2026 (5 Days Overdue)
    Lib->>Web: Scan Book Return
    Web->>API: POST /api/v1/library/loans/{id}/return (condition: 'GOOD')
    API->>API: Calculate Overdue Days (5 days * ETB 5/day = ETB 25.00)
    API->>DB: Mark book_loan 'RETURNED', Increment available_copies
    API->>Fee: Post Library Fine to Student Balance Statement (ETB 25.00)
    Fee->>DB: Insert fee_statement_line (Type: 'LIBRARY_FINE')
    API-->>Web: Return Processed. Overdue Fine: ETB 25.00 appended to fee account.
```

---

## 8. Laboratory & Specialized Facility Reservation & Safety Management

```mermaid
graph TD
    A[Teacher Submits Lab Session Booking Request] --> B{Collision Engine Check}
    B -->|Time/Room Conflict| C[Reject Booking & Show Alternate Period Slots]
    B -->|Zero Conflicts| D[Lab Technician Reviews Apparatus & Reagent Availability]
    D --> E[Approve Booking & Reserve Chemical Safety Gear]
    E --> F[Class Enters Lab: Mandatory Safety Checklist Verified]
    F --> G[Practical Session Conducted]
    G --> H[Session Wrap-Up & Inspection]
    H --> I{Equipment Damage or Breakage?}
    I -->|Yes| J[Log Incident Report & Assess Breakage Replacement Fee]
    I -->|No| K[Restock Clean Equipment & Complete Session Log]
```

---

## 9. Student Clinic Visit, Triage & Guardian Emergency Escalation

```mermaid
sequenceDiagram
    autonumber
    actor Student as Sick / Injured Student
    actor Nurse as School Health Officer / Nurse
    participant Web as Clinic Web Portal
    participant API as Health Module
    participant DB as PostgreSQL (RLS)
    participant Comms as Notification Service
    actor Parent as Student Guardian

    Student->>Nurse: Report to School Clinic / Dispensary
    Nurse->>Web: Open Student Medical Profile (Check Blood Group, Allergies, Asthma)
    Nurse->>Web: Log Vitals: Temp 39.1°C, Symptoms: Acute Fever & Vomiting
    Nurse->>Web: Set Triage Severity: 'SEVERE' & Assign Rest Bed #2
    Web->>API: POST /api/v1/health/clinic-visits (escalate_urgent: true)
    API->>DB: Insert health_clinic_visit & medical notes
    API->>Comms: Dispatch High-Priority Emergency Event
    DB-->>API: Committed
    API-->>Web: Visit Recorded & Escalation Dispatched
    
    Comms->>Parent: Send Urgent SMS: "[URGENT - Fidel Hub Clinic] Your child Dawit Bekele has reported to the clinic with high fever (39.1°C). Please call 0911-XXXXXX or proceed to school."
```

---

## 10. Staff/Teacher Grade Modification & Audit Trail

```mermaid
sequenceDiagram
    autonumber
    actor Teacher as Classroom Teacher
    participant Web as Gradebook Marksheet UI
    participant API as Grading Module
    participant Audit as Teacher Audit Engine
    participant DB as PostgreSQL (RLS)
    actor Principal as School Principal

    Teacher->>Web: Change Student Mark from 78 to 86 in Grade 10 Physics
    Web->>API: PUT /api/v1/grades/marksheet/entry<br/>(student_id, subject_id, old_score: 78, new_score: 86, reason: 'Calculation Correction')
    
    API->>DB: Check Term Gradebook Lock Status
    alt Gradebook Already Locked (Post-Deadline)
        API-->>Web: 403 Forbidden ("Gradebook locked by Principal on 28 Oct 2026. Modification requires Administrative Override.")
        Teacher->>Principal: Request Post-Deadline Score Amendment
        Principal->>API: POST /api/v1/grades/override-unlock (with authorization token)
    end

    API->>DB: Update student assessment score
    API->>Audit: Record Audit Log (staff_user_id, student_id, old_score: 78, new_score: 86, reason, IP, timestamp)
    Audit->>DB: INSERT INTO audit.teacher_access_logs (...)
    DB-->>API: Transaction Committed
    API-->>Web: Grade Updated & Audit Trail Permanently Recorded
```

---

## 11. SaaS Platform SuperAdmin Tenant Onboarding & Quota Lifecycle

```mermaid
sequenceDiagram
    autonumber
    actor Super as Platform SuperAdmin
    participant UI as SuperAdmin Control Panel
    participant API as Platform Management API
    participant DB as PostgreSQL (RLS)
    actor SchoolAdmin as School Tenant Administrator

    Super->>UI: Register New School Tenant: "Fidel Hub Academy" (Tier: 'STANDARD', Student Cap: 1,500)
    UI->>API: POST /api/v1/platform/tenants
    API->>DB: INSERT INTO platform.tenants (code: 'fha', name: 'Fidel Hub Academy', status: 'TRIAL')
    API->>DB: INSERT INTO platform.tenant_quotas (max_students: 1500, max_branches: 3)
    API->>DB: Provision Default Main Branch & School Calendar
    API-->>UI: Tenant Provisioned Successfully
    
    Note over SchoolAdmin,API: Quota Enforcement Invariant
    SchoolAdmin->>API: Bulk Admit 200 Students (Current Active: 1,450)
    API->>DB: Count current active students + 200
    alt Count exceeds 1,500 quota
        API-->>SchoolAdmin: 402 Payment Required ("Enrollment limit of 1,500 reached for Standard Plan. Please upgrade to Enterprise Tier.")
    else Count within quota
        API->>DB: Commit Student Admissions
        API-->>SchoolAdmin: Admissions Completed
    end

    Note over Super,DB: Tenant Backup & Sovereignty Export
    Super->>API: POST /api/v1/platform/tenants/{id}/backup-export
    API->>DB: Execute tenant-isolated pg_dump (RLS filtered by tenant_id)
    API->>API: Encrypt archive with AES-256 and store in secure storage
    API-->>Super: Signed Download Link (Valid for 2 hours)
```
