# ADR-004: Offline-First Computer-Based Testing (CBT) & Local Exam Venue Server

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
Schools in East Africa frequently suffer from power outages and intermittent internet connectivity. Conducting Computer-Based Testing (CBT) and term examinations cannot depend on an active cloud internet connection during exam sessions. If connectivity drops during an exam sitting, students must not lose their progress or be locked out.

## 2. Decision Outcome
Chosen: **Offline-First CBT Architecture with Encrypted Exam Packages & Local Exam Venue Server (SQLite WAL)**.

### Architecture:
1. **Encrypted Package Export**: Central cloud API packages questions, options, and diagrams into an AES-256-GCM bundle with a SHA-256 integrity checksum.
2. **Local Exam Venue Server**: A lightweight service running on the school LAN (laptop/mini-PC) pre-downloads the bundle and time-locks it until the scheduled `exam_start_at`.
3. **LAN-Only Exam Sitting (Zero Internet Required)**: Students connect directly to the venue server's local IP address or take tests locally in Expo SQLite in Write-Ahead Logging (`WAL`) mode.
4. **Continuous Autosave**: Every answer change and remaining timer state is autosaved locally with `is_final = false`.
5. **Idempotent Background Synchronization**: When internet connectivity returns, sessions and answers are synced to the central cloud via `POST /api/v1/cbt/sessions/sync` using unique composite constraints (`ON CONFLICT (session_id, question_id)`), guaranteeing zero duplicate records or score skews.
