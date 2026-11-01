# ACCEPTANCE CRITERIA SPECIFICATION

## Fidel Hub — Functional Verification & User Story Acceptance Matrix

| Document Information | Details |
| :--- | :--- |
| **Document ID** | ACC-001 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Format** | Behavioral Driven Specification (Given-When-Then / Gherkin) |
| **Coverage** | Core Operational & Mission-Critical Workflows Across All 15 Modules |

---

## 1. Student SIS, Admissions & Enrollment

### AC-SIS-001: Three-Part Ethiopian Name Validation
* **Given** an admissions officer is registering a new student,
* **When** they submit first name, father's name, and grandfather's name,
* **Then** the system concatenates the full name correctly and verifies that no required patronymic component is missing.

### AC-SIS-002: Unique Branch Admission Number
* **Given** an approved admission application,
* **When** the registrar executes the approval action,
* **Then** the system automatically generates a unique sequential admission number (e.g. `FHA-2026-0042`) scoped to the branch, inserts student records, and creates an initial fee assignment snapshot.

---

## 2. Academics, Curriculum & Timetable

### AC-ACAD-001: Timetable Room & Teacher Collision Prevention
* **Given** Teacher Abebe is scheduled to teach Physics in Section 9-A on Tuesday Period 3,
* **When** an administrator attempts to assign Teacher Abebe to Section 9-B during the exact same period,
* **Then** the system rejects the modification with HTTP 409 Conflict, highlighting the schedule collision.

### AC-ACAD-002: Early Childhood / Nursery Daily Activity Log
* **Given** a kindergarten teacher is managing the Nursery section,
* **When** the teacher submits meal intake, nap duration, and potty logs,
* **Then** the record is persisted and immediately surfaced on the Parent Mobile Portal under the child's daily activity digest.

---

## 3. Daily Attendance Tracking

### AC-ATT-001: Rapid Roster Entry
* **Given** a homeroom teacher opens the morning attendance roster for Section 8-A,
* **When** the roster loads,
* **Then** all students default to `PRESENT`, allowing the teacher to mark unexcused absences and tardiness in under 30 seconds.

### AC-ATT-002: Automated Parent Truancy Notification
* **Given** a student is marked as `ABSENT (UNEXCUSED)` by the homeroom teacher,
* **When** the session is submitted,
* **Then** an asynchronous event is written to `integration.outbox_messages`, and an automated SMS alert is dispatched to the primary guardian's phone number within 15 minutes.

---

## 4. Question Bank & Offline CBT Examination Engine

### AC-CBT-001: Time-Locked Exam Package Security
* **Given** an exam coordinator exports an exam paper bundle for an assessment scheduled at 09:00 AM tomorrow,
* **When** the package is downloaded onto the Local Venue Server,
* **Then** the file is stored as an AES-256-GCM encrypted payload, and any attempt to decrypt the paper prior to 09:00 AM or without coordinator 2FA authentication is blocked.

### AC-CBT-002: Zero-Internet LAN Sitting & Continuous Autosave
* **Given** a student is actively taking a 60-minute exam on the school LAN with external internet completely disconnected,
* **When** the student selects an answer option,
* **Then** the response is autosaved to the Local Venue Server's SQLite WAL database in under 50ms without network errors.

### AC-CBT-003: Workstation Crash Recovery
* **Given** a student has answered 25 questions and their computer experiences a hardware crash,
* **When** the student logs into a replacement computer using their Admission Number and PIN,
* **Then** all 25 previously answered questions are fully restored, and the countdown timer resumes at the exact remaining duration.

### AC-CBT-004: Idempotent Batch Sync Deduplication
* **Given** a Local Venue Server re-transmits a completed 200-student exam batch due to an unstable internet connection,
* **When** the Fidel Hub Central API ingests the batch multiple times with the same `batch_token`,
* **Then** the database enforces `ON CONFLICT (session_id, question_id) DO NOTHING`, ensuring zero duplicate rows and zero mark distortion.

---

## 5. Grading, Marksheets & Report Cards

### AC-GRD-001: Term Gradebook Locking & Post-Deadline Modification Audit
* **Given** the term gradebook marksheet has been officially locked by the School Principal,
* **When** a teacher attempts to edit a student's score,
* **Then** the write is rejected; if an administrative override is granted, the action generates a tamper-alert log in `audit.teacher_access_logs` recording the old score, new score, staff ID, and reason.

### AC-GRD-002: Dual-Script PDF Report Card Verification
* **Given** term grades are finalized,
* **When** report cards are compiled in batch,
* **Then** the output PDF renders clean English and Amharic Ge'ez typography and includes a unique verification QR code leading to `https://verify.fidelhub.com/transcripts/{hash}`.

---

## 6. Student Fee Management & Invoicing (No General Ledger)

### AC-FIN-001: Posted Invoice Immutability
* **Given** an invoice has transitioned from `DRAFT` to `POSTED`,
* **When** any user attempts an in-place SQL update or deletion of the line items or total due,
* **Then** the database trigger aborts the transaction with an exception stating posted invoices are immutable.

### AC-FIN-002: Payment Idempotency Guarantee
* **Given** a cashier submits a payment of ETB 4,500.00 with `Idempotency-Key: {UUID}`,
* **When** a network glitch causes the request to be delivered twice,
* **Then** the second request returns the existing official receipt without decrementing the student balance a second time.

---

## 7. Library Circulation & Facilities

### AC-LIB-001: Overdue Fine Accrual
* **Given** a student borrows a book with a 14-day loan policy and returns it 5 days late,
* **When** the librarian scans the return in the portal,
* **Then** the system calculates the overdue fee (5 days × daily rate), marks the loan returned, and appends the fine directly to the student fee balance statement.

### AC-FAC-001: Lab Room Reservation Collision Prevention
* **Given** the Chemistry Lab is reserved for Section 10-A on Monday Period 2,
* **When** a Biology teacher attempts to book the Chemistry Lab for Section 11-B during that same period,
* **Then** the reservation is rejected with an explicit room collision warning.

---

## 8. Student Health, Clinic & Emergencies

### AC-HLT-001: Emergency Clinic Visit Triage Alert
* **Given** a student reports to the clinic with severe symptoms (e.g. 39.5°C fever or trauma),
* **When** the school nurse submits the visit with triage severity set to `SEVERE`,
* **Then** the system persists the clinic visit and immediately triggers an urgent SMS alert to the student's primary guardian.

---

## 9. Platform SuperAdmin & Quotas

### AC-PLT-001: Student Enrollment Plan Quota Enforcement
* **Given** a school tenant is on the Standard Subscription Tier with an enrollment limit of 1,500 students and currently has 1,500 active students,
* **When** a registrar attempts to admit student #1,501,
* **Then** the system halts admission with HTTP 402 Payment Required, directing the school administrator to upgrade to Enterprise Tier.
