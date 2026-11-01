# API SPECIFICATION & CONVENTIONS

## Fidel Hub — REST & OpenAPI Contracts

| Document Information | Details |
| :--- | :--- |
| **Document** | API Specification & Conventions |
| **Version** | 1.0-DRAFT |

---

## 1. Global API Conventions
- **Base URI**: `/api/v1/`
- **Tenant Context**: Subdomain (e.g., `https://stjoseph.fidelhub.com/api/v1/...`) or header `X-Tenant-ID: <uuid>`.
- **Response Format**:
  ```json
  {
    "isSuccess": true,
    "data": { ... },
    "error": null,
    "timestamp": "2026-11-01T12:00:00Z"
  }
  ```
- **Error Format (RFC 7807 Problem Details)**:
  ```json
  {
    "type": "https://fidelhub.com/errors/validation-failed",
    "title": "Validation Error",
    "status": 400,
    "errors": { "admissionNumber": ["Admission number already exists in this school."] }
  }
  ```

---

## 2. Core Endpoints Summary

### 2.1 Students & Admissions
- `GET /api/v1/students?page=1&pageSize=25&branchId={id}`
- `POST /api/v1/students`
- `GET /api/v1/students/{id}`
- `PUT /api/v1/students/{id}`

### 2.2 Attendance
- `POST /api/v1/attendance/sessions` (Batch attendance submission)
- `GET /api/v1/attendance/summary?classId={id}&date={date}`

### 2.3 Finance & Fees
- `POST /api/v1/finance/invoices/generate`
- `POST /api/v1/finance/payments` (Requires `Idempotency-Key` header)
- `GET /api/v1/finance/receipts/{receiptId}`

### 2.4 Mobile Sync
- `POST /api/v1/sync/push` (Flush pending operations)
- `GET /api/v1/sync/delta?since={timestamp}` (Pull recent updates)
