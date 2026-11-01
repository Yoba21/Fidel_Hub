# ADR-004: Offline-First Computer-Based Testing (CBT) & Exam Delivery

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted (Revised per PRD Scope) |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
Schools in Ethiopia frequently suffer from unpredictable power outages and network drops. Conducting Computer-Based Testing (CBT) and term examinations cannot rely on a constant active internet connection during exam sessions. If connectivity drops during a 2-hour assessment, students must not lose their progress or be locked out of the exam.

## 2. Decision Outcome
Chosen: **Offline-First CBT (Computer-Based Testing) & Exam Synchronization Engine**.

### Architecture & Mechanisms:
1. **Encrypted Local Exam Package**:
   - Before an exam session commences, student devices or classroom lab computers download the encrypted assessment bundle (questions, multiple choice options, diagrams, time limits, and session token).
   - Once downloaded, the test runs completely locally in Expo SQLite / local storage without needing internet access.
2. **Local Session Heartbeat & Progress Auto-Save**:
   - Student answers, elapsed time, question navigation, and option selections are committed immediately to the local database after every interaction.
3. **Session Finalization & Submission Queue**:
   - When the student finishes or the timer expires, the completed exam session bundle is signed locally and placed in the local `sync_queue`.
4. **Idempotent Background Synchronization**:
   - As soon as the device reconnects to the school Wi-Fi or local cache server, the answers are uploaded via `POST /api/v1/cbt/sessions/submit` using client `session_id` as an idempotency key.
   - Server-side validation guarantees zero duplicate marks and records exact submission timestamps.
