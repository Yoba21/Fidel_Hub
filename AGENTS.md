# Fidel Hub Architectural Invariants & Audit Gates

All code generation, module implementations, and database changes in Fidel Hub must satisfy these non-negotiable invariants:

## 1. Pre-Implementation Audit Gate
Before authoring new features or domain modules:
- Always compare the planned implementation against the corresponding canonical specification in `docs/`.
- Verify that shared kernel interfaces (`ITenantScoped`, `IBranchScoped`, `IAuditableEntity`) and base entities match the documented models.
- Never write domain logic until structural dependencies and configuration keys are harmonized.

## 2. Multi-Tenancy & Database Storage Invariants
- **Primary & Secondary Keys**: Every tenant-scoped table MUST feature a single-column primary key (`PRIMARY KEY (id)`) paired with `UNIQUE (tenant_id, id)` (and `UNIQUE (tenant_id, branch_id, id)` if branch-scoped).
- **Composite Foreign Keys**: Child tenant tables must reference parent tables using composite foreign keys: `FOREIGN KEY (tenant_id, parent_id) REFERENCES parent_table (tenant_id, id)`.
- **Connection & Transaction Session Scoping**: The database interceptor MUST establish tenant session context on connection checkout (`set_config('app.current_tenant_id', @tenantId, false)`) and MUST reset it upon connection return (`set_config('app.current_tenant_id', '', false)`) to prevent connection pool leakage. Within explicit database transactions (`TransactionStarted`), it MUST enforce transaction-local scoping (`is_local = true`).
- **Fail Closed**: Database interceptors must never fail open. If tenant session scoping fails to apply, the connection or query MUST be denied immediately.
- **Unprivileged Application Role**: All application database operations must execute under `fidel_app_user` (`NOSUPERUSER`, `NOBYPASSRLS`).

## 3. Authentication & Token Invariants
- **Asymmetric Tokens**: Authentication relies on the Better Auth microservice issuing RS256/EdDSA JWTs with JWKS key rotation at `/.well-known/jwks.json`.
- **API Token Validation**: ASP.NET Core `JwtBearer` must validate tokens against the Better Auth JWKS endpoint or authority; never use symmetric HMAC secret keys for Better Auth JWT validation.
- **Context Claims**: Tokens must include `tenant_id`, `branch_id`, `roles`, and `permissions`, mapped into `ICurrentTenantContext`.

## 4. Frontend & Ecosystem Alignment
- Form validations must use Zod schemas paired with React Hook Form.
- Client UI styling must strictly use theme tokens from `fidelTheme` (`#1B5E3F`, `#E0A526`, `#FAF7F0`, `#1F2933`).
- Local storage for offline CBT operations must configure SQLite in WAL mode (`PRAGMA journal_mode = WAL;`).
