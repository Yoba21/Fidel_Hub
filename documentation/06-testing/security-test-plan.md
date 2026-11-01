# SECURITY TEST PLAN

## Fidel Hub — Penetration & Security Verification

| Document Information | Details |
| :--- | :--- |
| **Document** | Security Test Plan |
| **Version** | 1.0-DRAFT |

---

## 1. Security Vectors
- **Tenant Context Spoofing**: Attempting to alter `X-Tenant-ID` header with mismatched JWT token signature.
- **RLS Bypass via Raw SQL / Dapper**: Verifying that queries without manual WHERE filters are still restricted by PostgreSQL kernel RLS.
- **SQL Injection & XSS**: Automated static and dynamic analysis (SAST/DAST) across all API query inputs and form fields.
- **Idempotency Replay**: Replaying duplicate payment payloads to verify rejection and single ledger creation.
