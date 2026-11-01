# PRODUCTION READINESS CHECKLIST

## Fidel Hub — Launch Verification Checklist

| Document Information | Details |
| :--- | :--- |
| **Document** | Production Checklist |
| **Version** | 1.0-DRAFT |

---

## Pre-Launch Checks
- [ ] Database credentials rotated and application connects as non-superuser idel_app_user.
- [ ] PostgreSQL Row-Level Security verified across all tenant tables via integration tests.
- [ ] SSL/TLS certificates configured with HTTPS redirection and HSTS headers.
- [ ] Transactional Outbox worker active and monitoring failed message queues.
- [ ] Double-entry ledger invariant checks ($\sum \text{Debit} = \sum \text{Credit}$) scheduled nightly.
- [ ] Offline sync retry limit and exponential backoff parameters verified.
