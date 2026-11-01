# TECHNICAL ARCHITECTURE DOCUMENT (TAD)

## Fidel Hub — School Management SaaS Platform

Document: TAD-003
Version: 1.0
Status: Architecture Baseline
Date: 1 November 2026

---

1. Introduction

1.1 Purpose

This document defines the technical architecture of the Fidel Hub School Management SaaS Platform.

It establishes how the system will be structured, how its modules will communicate, how tenant and branch data will be protected, and how the application will be developed, deployed, tested, and maintained.

The document serves as the technical reference for developers, technical leads, database engineers, QA engineers, and AI coding agents.

1.2 Architecture Goals

The architecture must:

1. Support multiple independent school organizations on one SaaS platform.
2. Support multiple independently administered branches under each tenant.
3. Protect tenant and branch data from unauthorized access.
4. Support future expansion without requiring a complete architectural redesign.
5. Keep the application maintainable through clear module boundaries.
6. Support web, mobile, and offline CBT examination workflows.
7. Maintain financial and academic data integrity.
8. Support Ethiopian and Gregorian calendar requirements.
9. Provide reliable APIs for web and mobile applications.

1.3 Architecture Principles

- Modular monolith before microservices.
- Shared PostgreSQL database and shared tables.
- Tenant isolation enforced at both application and database levels.
- Branch-level access and financial separation.
- Business logic belongs in the backend, not the frontend.
- Modules own their business rules and data.
- Cross-module communication should use defined interfaces.
- Financial transactions must be consistent and auditable.
- APIs must be versioned and documented.
- Security must be enforced by default.
- Offline functionality is focused on CBT assessments and examinations in the initial release.

2. Technology Stack

The following stack is the approved technical baseline.

Layer| Technology
Backend| ASP.NET Core 10 Web API
Language| C#
ORM| Entity Framework Core 10
Database provider| Npgsql for EF Core
Database| PostgreSQL
Web frontend| React + TypeScript + Vite
Mobile application| React Native + Expo
Offline storage| SQLite
API style| REST
Authentication| Secure token-based authentication
Authorization| Role-based and permission-based access
Database isolation| PostgreSQL Row-Level Security
Background processing| .NET Hosted Services
API documentation| OpenAPI
Architecture| Modular monolith

Package versions must remain compatible with the selected .NET and EF Core major versions. The Npgsql provider version must be verified against the selected EF Core release before implementation.

3. High-Level System Architecture

The system consists of three primary application components:

1. Web application.
2. Mobile application.
3. Backend API.

Both client applications communicate with the same backend API. The backend controls business rules, authorization, tenant context, database access, and background processing.

                    FIDEL HUB PLATFORM   
                            |
             +--------------+--------------+
             |                             |
       WEB APPLICATION              MOBILE APPLICATION
       React + TypeScript            React Native + Expo
             |                             |
             +--------------+--------------+
                            |
                       REST API
                            |
                  ASP.NET CORE 10
                            |
                 APPLICATION LAYER
                            |
          +-----------------+------------------+
          |                 |                  |
     PLATFORM          ACADEMIC             FINANCE
     MANAGEMENT        MANAGEMENT           MANAGEMENT
[01/11/2026 12:33 PM] Yo: |                 |                  |
     TENANT/BRANCH      STUDENTS             FEES
     USERS              ADMISSIONS           INVOICES
     ACCESS             ENROLLMENT           PAYMENTS
          |                 |                  |
          +-----------------+------------------+
                            |
                    INFRASTRUCTURE
                            |
               ENTITY FRAMEWORK CORE 10
                            |
                       POSTGRESQL
                            |
                ROW-LEVEL SECURITY (RLS)

The diagram represents logical responsibilities, not separate deployable services.

All backend modules run inside one ASP.NET Core application and share one PostgreSQL database.

4. Backend Architecture

4.1 Architecture Style

Use a modular monolith with clear module boundaries.

A modular monolith provides the simplicity of one deployable backend while maintaining separation between business domains.

The initial system must not be divided into microservices.

Reasons:

- Simpler deployment and maintenance.
- Easier database transactions.
- Lower infrastructure complexity.
- Easier development for a small or medium-sized team.
- Clear module boundaries still allow future extraction of services if necessary.

4.2 Backend Layers

Each business module should follow these logical layers:

Presentation Layer

- API controllers or endpoints.
- Request and response models.
- Authentication and authorization requirements.
- Input validation.
- HTTP response handling.

Application Layer

- Use cases.
- Commands and queries.
- Application services.
- Business workflow orchestration.
- Authorization checks.
- Transaction coordination.

Domain Layer

- Business entities.
- Value objects.
- Business rules.
- State transitions.
- Domain events where necessary.

Infrastructure Layer

- EF Core database access.
- Repository implementations where justified.
- External integrations.
- File storage.
- Background processing.
- Email and notification providers.

Not every operation needs a repository abstraction. Use EF Core directly through well-defined application services where that keeps the implementation simpler.

4.3 Proposed Backend Solution Structure

school-management/
|
+-- backend/
|   |
|   +-- SchoolManagement.sln
|   |
|   +-- src/
|   |   |
|   |   +-- SchoolManagement.Api/
|   |   |   +-- Controllers/
|   |   |   +-- Middleware/
|   |   |   +-- Filters/
|   |   |   +-- Configuration/
|   |   |   +-- Program.cs
|   |   |
|   |   +-- BuildingBlocks/
|   |   |   +-- Authentication/
|   |   |   +-- Authorization/
|   |   |   +-- TenantContext/
|   |   |   +-- BranchContext/
|   |   |   +-- Persistence/
|   |   |   +-- Auditing/
|   |   |   +-- Messaging/
|   |   |   +-- Validation/
|   |   |   +-- Exceptions/
|   |   |
|   |   +-- Modules/
|   |       |
|   |       +-- Identity/
|   |       +-- PlatformManagement/
|   |       +-- TenantManagement/
|   |       +-- BranchManagement/
|   |       +-- SchoolConfiguration/
|   |       +-- StudentManagement/
|   |       +-- Admissions/
|   |       +-- AcademicManagement/
|   |       +-- StaffManagement/
|   |       +-- Attendance/
|   |       +-- Examination/
|   |       +-- FeeManagement/
|   |       +-- Reporting/
|   |       +-- Audit/
|   |
|   +-- tests/
|       +-- UnitTests/
|       +-- IntegrationTests/
|       +-- ArchitectureTests/
|       +-- ApiTests/
|
+-- frontend/
|
+-- mobile/
|
+-- database/
|
+-- documentation/

Each module may contain its own:

ModuleName/
    Domain/
    Application/
    Infrastructure/
    Presentation/

A module can have fewer folders where the additional separation would add no value. The important requirement is ownership and separation of responsibilities, not folder count.

5. Module Boundaries and Responsibilities

5.1 Identity and Access Management

Responsible for:

- User accounts.
- Authentication.
- Password management.
- User activation and deactivation.
- Role definitions.
- Permission definitions.
- User-to-role assignments.
- Session and token management.
[01/11/2026 12:33 PM] Yo: It must not own student, fee, examination, or attendance business data.

5.2 Platform Management

Responsible for:

- Platform administration.
- Tenant registration and activation.
- Tenant suspension.
- Platform-level configuration.
- Platform-level operational monitoring.

Platform administrators must not receive unrestricted access to school data by default.

Any authorized support access must be explicit and audited.

5.3 Tenant and Branch Management

Responsible for:

- Tenant profiles.
- Branch creation and configuration.
- Branch activation and suspension.
- Branch access assignments.
- Branch-specific settings.
- Branch-level operational boundaries.

A tenant represents the school organization. A branch represents an independently administered and financially managed campus.

5.4 School Configuration

Responsible for:

- Academic settings.
- Grading configuration.
- Class and grade configuration.
- Section configuration.
- School calendar settings.
- Ethiopian and Gregorian calendar preferences.
- Configurable school-specific settings.

5.5 Student Management

Responsible for:

- Student identity.
- Student profiles.
- Guardian relationships.
- Student documents.
- Student status.
- Student history.

Student identity must remain separate from branch-specific enrollment.

5.6 Admissions

Responsible for:

- Admission applications.
- Application review.
- Approval and rejection.
- Admission documents.
- Admission history.
- Admission-to-enrollment handover.

Admission approval must not automatically bypass academic enrollment rules.

5.7 Academic Management

Responsible for:

- Academic years.
- Terms.
- Grades and classes.
- Sections.
- Student enrollment.
- Class assignments.
- Academic progression.
- Student transfers.

5.8 Staff Management

Responsible for:

- Staff profiles.
- Teacher profiles.
- Staff assignments.
- Teacher-to-class assignments.
- Staff status and history.

Payroll and advanced HR functionality are outside the initial release.

5.9 Attendance

Responsible for:

- Attendance sessions.
- Student attendance records.
- Attendance submission.
- Attendance corrections.
- Attendance history.
- Attendance reporting.

The attendance module owns the attendance rules and state transitions.

5.10 Examination & Offline CBT

Responsible for:

- Offline Computer-Based Testing (CBT) package distribution and offline exam taking.
- Examination configuration.
- Assessment structure.
- Marks entry.
- Result calculation.
- Result review and approval.
- Result publication.
- Academic result history.

Published results must not be silently overwritten.

5.11 Fee Management

Responsible for:

- Fee categories.
- Fee structures.
- Student fee assignments.
- Invoices.
- Payments.
- Payment allocations.
- Refunds.
- Outstanding balances.
- Financial reporting.

All fee and payment records must be associated with the appropriate tenant and branch.

5.12 Reporting and Audit

Reporting provides authorized read-only access to operational information.

Audit records important actions, including:

- User and permission changes.
- Student record changes.
- Attendance corrections.
- Examination approvals and publication.
- Invoice posting.
- Payment confirmation.
- Refund approval.
- Branch configuration changes.

Audit records must not be editable through normal application operations.

6. Module Dependency Rules

Modules must not directly modify another module's database records.

The following rules apply:

1. Each module owns its business entities and rules.
2. A module can read another module's data through an approved application interface.
3. Cross-module operations must use explicit application services or internal events.
4. Shared building blocks must contain technical capabilities, not school business rules.
5. Avoid circular dependencies.
6. Do not allow controllers to directly access another module's internal DbContext or tables.

Example:

When a student is admitted and approved, the Admissions module coordinates with Student Management and Academic Management through their application interfaces.
[01/11/2026 12:33 PM] Yo: It must not directly insert records into their internal tables without going through the defined business operations.

7. Multi-Tenant Architecture

7.1 Tenant Model

The database uses a shared database and shared tables.

Each tenant-owned record must contain a "tenant_id".

Branch-owned records must also contain a "branch_id".

Tenant
  |
  +-- Branch A
  |     |
  |     +-- Students
  |     +-- Enrollments
  |     +-- Attendance
  |     +-- Fees
  |
  +-- Branch B
        |
        +-- Students
        +-- Enrollments
        +-- Attendance
        +-- Fees

7.2 Tenant Context Resolution

The backend must resolve the tenant from authenticated user access and validated request context.

The system must never trust a tenant ID supplied by the frontend without authorization validation.

For each request:

1. Authenticate the user.
2. Identify the user's tenant membership.
3. Validate the requested tenant context.
4. Validate branch access when branch context is required.
5. Establish the tenant and branch context for database operations.
6. Execute the application operation.
7. Clear the database context automatically at transaction completion.

7.3 Tenant Context Service

Create a shared service such as:

public interface ITenantContext
{
    Guid TenantId { get; }
    Guid UserId { get; }
    Guid? BranchId { get; }
}

The implementation must obtain values from validated authentication and authorization context.

Do not allow ordinary application services to freely change tenant context.

7.4 PostgreSQL Row-Level Security

PostgreSQL RLS provides a second layer of tenant and branch isolation.

Use transaction-local database settings:

SELECT set_config('app.tenant_id', 'tenant-uuid', true);
SELECT set_config('app.user_id', 'user-uuid', true);
SELECT set_config('app.branch_id', 'branch-uuid', true);

The final argument "true" makes each setting transaction-local.

Policies must fail closed when required context is missing.

Conceptual tenant policy:

USING (
    tenant_id = current_setting(
        'app.tenant_id',
        true
    )::uuid
)

Branch policies must also verify that the user is authorized for the relevant branch or has a valid tenant-wide administrative permission.

The actual policies must handle missing settings safely rather than relying on unsafe casts or permissive defaults.

7.5 Database Role Security

The application database role must:

- Not be a PostgreSQL superuser.
- Not own the protected tables.
- Not have "BYPASSRLS".
- Have only the permissions required by the application.
- Use RLS-protected access for tenant-owned data.

Enable and evaluate "FORCE ROW LEVEL SECURITY" on protected tables where appropriate.

RLS is a security control, not a replacement for application-level authorization.

7.6 EF Core and Transaction Context

All tenant-scoped database operations must establish database context inside the same transaction used for the operation.

A recommended execution pattern:

Begin database transaction
        |
Set transaction-local tenant context
        |
Set transaction-local user context
        |
Set branch context when required
        |
Execute EF Core queries and updates
        |
Commit or rollback
        |
Transaction ends and context is cleared

Do not set tenant context as a persistent connection-level setting. Database connections may be pooled and reused by different users.

Any operation that accesses tenant-owned records without a valid tenant context must fail.

8. Authentication and Authorization Architecture

8.1 Authentication

Use secure token-based authentication for web and mobile clients.

The implementation must support:

- Login.
- Logout and session revocation.
- Password reset.
- Password change.
- Account activation and deactivation.
- Token expiration.
- Refresh token rotation.
- Secure token storage.

For web, use a secure cookie-based approach where practical, with appropriate CSRF protection.

For mobile, use secure device storage for refresh credentials.

Access tokens should be short-lived. Refresh tokens must be revocable and stored securely.
[01/11/2026 12:33 PM] Yo: 8.2 Authorization

Use role-based access control combined with granular permissions.

Example:

Role: Branch Administrator

Permissions:
- student.view
- student.create
- student.update
- admission.view
- admission.approve
- attendance.view
- fee.view
- fee.report

Authorization must consider:

1. User identity.
2. Tenant membership.
3. Branch access.
4. Assigned role.
5. Required permission.
6. Resource ownership and scope.

A user having a permission does not automatically grant access to records outside their authorized tenant or branch.

8.3 Authorization Enforcement

Authorization must be enforced on the backend.

Frontend navigation restrictions are only a usability feature and must never be treated as security controls.

Sensitive operations such as payment confirmation, refunds, examination approval, and result publication require explicit permission checks.

9. Database Architecture

9.1 Database Design

Use PostgreSQL with shared tables.

Recommended schema separation:

platform
identity
school
students
admissions
academic
staff
attendance
examinations
finance
audit
integration

Schemas are logical organizational boundaries. They do not replace module ownership or authorization.

9.2 Primary Keys

Use UUID primary keys for application entities.

Tenant-owned entities must include "tenant_id".

Branch-owned entities must include both "tenant_id" and "branch_id".

9.3 Referential Integrity

Use database constraints to prevent invalid relationships.

Examples:

- A branch must belong to its tenant.
- An enrollment must reference a student belonging to the same tenant.
- An enrollment's class and section must belong to the same branch.
- A fee assignment must reference an enrollment belonging to the same student and branch.
- An assessment must belong to the correct academic year and term.
- Attendance must reference a valid student enrollment and attendance session.

Where necessary, use composite unique constraints and composite foreign keys to enforce these relationships.

Do not depend exclusively on application validation for relational integrity.

9.4 Financial Data

Use PostgreSQL "NUMERIC(14,2)" for monetary values unless a specific financial requirement justifies a different precision.

Financial rules:

- Confirmed payments only reduce outstanding balances.
- Payment allocations cannot exceed the payment amount.
- Payment allocations cannot exceed the outstanding invoice balance.
- Refunds cannot exceed the refundable amount.
- Posted invoices must not be silently edited.
- Financial transactions must be branch-specific.
- Concurrent payment and allocation operations must be protected by database transactions and appropriate locking.

The initial finance module covers school fees and collections, not full general ledger accounting.

9.5 EF Core Usage

Use EF Core 10 for:

- Entity mapping.
- Relationships.
- Query execution.
- Transactions.
- Concurrency handling.
- Database migrations.

Use explicit configurations for important entity relationships, indexes, unique constraints, and composite foreign keys.

Avoid exposing EF Core entities directly through API responses. Use request and response DTOs.

Use raw SQL selectively when required for PostgreSQL-specific features such as RLS policies, specialized indexes, and database functions.

10. API Architecture

10.1 REST API

The backend exposes REST endpoints to web and mobile clients.

Use a consistent API prefix:

/api/v1/

Example resource groups:

/api/v1/auth
/api/v1/tenants
/api/v1/branches
/api/v1/users
/api/v1/students
/api/v1/admissions
/api/v1/academic-years
/api/v1/enrollments
/api/v1/staff
/api/v1/attendance
/api/v1/examinations
/api/v1/fees
/api/v1/invoices
/api/v1/payments
/api/v1/reports

10.2 API Standards

All APIs must follow these standards:

- Consistent HTTP methods and status codes.
- Request validation.
- Standardized error responses.
- Pagination for list endpoints.
- Filtering and sorting where appropriate.
- Explicit authorization requirements.
- Tenant and branch scope va