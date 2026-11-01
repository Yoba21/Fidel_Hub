# OFFLINE SYNCHRONIZATION SPECIFICATION: CBT & EXAMINATIONS

## Fidel Hub — Offline-First Exam Taking & Synchronization Protocol

| Document Information | Details |
| :--- | :--- |
| **Document** | Offline Synchronization Specification |
| **Version** | 2.0-ALIGNED |
| **Primary Domain** | CBT (Computer-Based Testing) & Student Assessments |

---

## 1. Context & Motivation
In accordance with the PRD and school infrastructure realities, offline capability is critical for **Computer-Based Testing (CBT) and Examinations**. Network disruptions during an ongoing test must never result in lost answers, failed submissions, or timer desynchronization.

---

## 2. Local Exam Storage (Expo SQLite / Client Cache)

### 2.1 Schema on Mobile / Lab Client
```sql
CREATE TABLE IF NOT EXISTS offline_cbt_exams (
    exam_id TEXT PRIMARY KEY,
    title TEXT NOT NULL,
    duration_minutes INTEGER NOT NULL,
    encrypted_package TEXT NOT NULL,
    downloaded_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS offline_cbt_sessions (
    session_id TEXT PRIMARY KEY,
    exam_id TEXT NOT NULL,
    student_id TEXT NOT NULL,
    started_at TEXT NOT NULL,
    completed_at TEXT,
    answers_json TEXT NOT NULL,
    is_submitted INTEGER NOT NULL DEFAULT 0,
    sync_status TEXT NOT NULL DEFAULT 'PENDING'
);
```

---

## 3. CBT Offline Lifecycle & Synchronization Flow

```mermaid
sequenceDiagram
    autonumber
    actor Student as Student / Proctor
    participant App as Mobile/Lab App (Offline Engine)
    participant LocalDB as Local SQLite
    participant API as School.Api (CBT Module)

    Note over Student,App: Pre-Exam: Online Prep
    Student->>App: Download Assigned Exam Package
    App->>API: GET /api/v1/cbt/exams/{id}/package
    API-->>App: Encrypted Exam Package (Questions & Rules)
    App->>LocalDB: Store package in offline_cbt_exams

    Note over Student,LocalDB: Exam Execution: Entirely Offline
    Student->>App: Start Exam (Network disconnected)
    App->>LocalDB: Create offline_cbt_sessions record (UUID)
    loop Answer Selection
        Student->>App: Answer Question / Move Next
        App->>LocalDB: Persist incremental answers & timer state
    end
    Student->>App: Submit Exam (or Timer Expires)
    App->>LocalDB: Mark session is_submitted = 1, sync_status = 'PENDING'

    Note over App,API: Post-Exam: Synchronization
    App->>App: Network connectivity restored
    App->>API: POST /api/v1/cbt/sessions/sync (Payload + session_id idempotency key)
    API->>API: Validate session, auto-grade answers, store results
    API-->>App: 200 OK (Sync confirmed, Server Timestamp)
    App->>LocalDB: Update sync_status = 'SYNCED'
```

---

## 4. Conflict & Fraud Prevention Rules
- **Anti-Tampering**: Session start time and end time are cryptographically bounded; questions are served with deterministic randomization seeds.
- **Single Submission Guarantee**: Server treats `session_id` as a unique database constraint. Replaying a sync request returns the previously accepted submission.
