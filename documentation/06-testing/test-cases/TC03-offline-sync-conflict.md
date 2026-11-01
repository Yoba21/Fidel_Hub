# TEST CASE TC-03: Offline CBT Exam Session Submission & Sync

| Metadata | Value |
| :--- | :--- |
| **Category** | Offline-First CBT Synchronization |
| **Reference** | PRD Section 5.11 & ADR-004 |

---

## 1. Precondition
- Student device has pre-downloaded the Term 1 Mathematics CBT Package.
- Device network interface is intentionally toggled offline (airplane mode).

## 2. Test Steps
1. Student starts the test; offline session ID is generated locally in SQLite.
2. Student completes all 30 questions offline.
3. Test time completes; application records answers and places completed payload in `offline_cbt_sessions` with `sync_status = 'PENDING'`.
4. Restore internet connectivity and trigger sync worker.
5. Inspect server database response and re-trigger sync.

## 3. Expected Result
- Server ingests session, grades responses, and returns HTTP 200 with confirmation.
- Client SQLite transitions session to `SYNCED`.
- Replaying the sync request is handled idempotently without duplicate record creation.
