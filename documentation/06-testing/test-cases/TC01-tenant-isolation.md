# TEST CASE TC-01: Cross-Tenant Data Isolation

- **Category**: Security / Multi-Tenancy
- **Precondition**: Two tenants provisioned: Tenant A (ID: 1111...) and Tenant B (ID: 2222...).
- **Test Steps**:
  1. Authenticate as Tenant A Administrator.
  2. Request GET /api/v1/students/{TenantB_StudentId}.
- **Expected Result**: HTTP 404 Not Found (or 403 Forbidden). PostgreSQL RLS completely hides Tenant B record.
