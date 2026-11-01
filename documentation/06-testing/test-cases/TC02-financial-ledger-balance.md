# TEST CASE TC-02: Double-Entry Ledger Zero-Sum Verification

- **Category**: Financial Integrity
- **Precondition**: Active student with pending tuition invoice of ETB 5,000.
- **Test Steps**:
  1. Post fee payment of ETB 5,000 via Cashier API.
  2. Query journal_entries for the resulting voucher ID.
- **Expected Result**: Total Debit sum equals Total Credit sum exactly (ETB 5,000.0000). Invoice balance transitions to 0.
