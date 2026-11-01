# TEST CASE TC-03: Offline Sync Queue & Conflict Resolution

- **Category**: Mobile Synchronization
- **Precondition**: Teacher device in airplane mode (disconnected).
- **Test Steps**:
  1. Record morning attendance for Section 8A.
  2. Save session locally.
  3. Re-enable network connectivity.
  4. Trigger sync worker.
- **Expected Result**: Batch operations successfully pushed to /api/v1/sync/push. Roster committed on server and parent SMS events triggered.
