# ADR-004: Offline-First Synchronization Strategy for Mobile

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
Schools in Ethiopia frequently experience intermittent or absent internet connectivity. Teachers must still be capable of taking morning attendance and entering grades without interruption.

## 2. Decision Outcome
Chosen **Client-Side SQLite Cache with Operation Queue & Delta Sync**.

### Architecture:
1. **Local Database**: React Native + Expo SQLite mirrors roster and class metadata.
2. **Operation Queue (`sync_queue`)**: Offline mutations generate UUID `operation_id` with payload and timestamp.
3. **Synchronization**:
   - Push: When online, client flushes queue in FIFO order to `/api/v1/sync/push`.
   - Pull: Client requests changesets via `/api/v1/sync/delta?since={timestamp}`.
4. **Conflict Policy**: Server-authority timestamp ordering.
