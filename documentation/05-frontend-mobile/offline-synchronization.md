# OFFLINE SYNCHRONIZATION SPECIFICATION

## Fidel Hub — Offline-First Protocol & Conflict Handling

| Document Information | Details |
| :--- | :--- |
| **Document** | Offline Synchronization Specification |
| **Version** | 1.0-DRAFT |

---

## 1. Local Queue Architecture (`sync_queue`)
Local SQLite table storing operations when offline:
```sql
CREATE TABLE IF NOT EXISTS sync_queue (
    operation_id TEXT PRIMARY KEY,
    module TEXT NOT NULL,
    action TEXT NOT NULL,
    payload TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'PENDING',
    retry_count INTEGER NOT NULL DEFAULT 0,
    created_at TEXT NOT NULL
);
```

---

## 2. Sync Protocol
1. **Push Phase**:
   - Flush pending items to `POST /api/v1/sync/push`.
   - Each item includes `operation_id` as the idempotency token.
   - Successful items transition to `SYNCED` and are pruned after 30 days.
2. **Pull Phase**:
   - Call `GET /api/v1/sync/delta?last_sync_timestamp={t}`.
   - Server returns modified entities scoped to the user's branch and assigned classes.
3. **Conflict Resolution Policy**:
   - Server timestamp wins on concurrent teacher edits.
   - Financial operations are strictly prohibited while offline.
