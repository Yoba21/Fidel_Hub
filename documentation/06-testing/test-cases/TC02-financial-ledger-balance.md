# TEST CASE TC-02: Fee Invoice Payment & Idempotency Verification

| Metadata | Value |
| :--- | :--- |
| **Category** | Fee & Payment Integrity |
| **Reference** | PRD Section 5.12 |

---

## 1. Precondition
- Active student enrolled under Branch A with an existing `POSTED` invoice of ETB 4,500.00.

## 2. Test Steps
1. Cashier initiates payment of ETB 4,500.00 with `Idempotency-Key: d290f1ee-6c54-4b01-90e6-d701748f0851`.
2. Verify API returns HTTP 201 Created with official sequential Receipt #.
3. Verify Invoice status transitions from `POSTED` to `PAID`, with `paid_amount = 4500.00`.
4. Replay the identical payment request using the exact same `Idempotency-Key`.

## 3. Expected Result
- The duplicate request returns HTTP 200 OK with the existing receipt and does not increment payment totals or duplicate invoice payment records.
