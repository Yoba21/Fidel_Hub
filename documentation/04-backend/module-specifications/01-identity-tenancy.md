# MODULE 01: IDENTITY & TENANCY SPECIFICATION

## Fidel Hub Backend Specification

| Module | Responsibility |
| :--- | :--- |
| **Identity & Tenancy** | Organization onboarding, branch management, subscription tiers, users, roles, and permissions. |

---

## 1. Domain Entities
- `Tenant`: ID, Name, Subdomain, LogoUrl, IsActive, SubscriptionPlan.
- `Branch`: ID, TenantId, Name, Code, City, Address, ContactPhone.
- `ApplicationUser`: ID, TenantId, Email, PhoneNumber, FullName, Status.
- `Role`: ID, TenantId, Name, NormalizedName.
- `Permission`: Code (e.g., `students.create`), Description.

## 2. Key Commands & Queries
- `RegisterTenantCommand`: Provisions a new school organization, creates default admin, and provisions demo chart of accounts.
- `CreateBranchCommand`: Adds a new campus under the tenant.
- `AuthenticateUserQuery`: Validates credentials, issues JWT token with tenant/branch claims.
