# Open Issues - Fidel Hub

*Last updated: after commit 72ff0ca (pushed to main)*

## Build & CI
- [x] **Uncommitted solution file changes** - `FidelHub.sln` normalized across all 20 projects with SDK GUIDs and forward-slash paths. Builds cleanly.
- [ ] **CI verification pending** - `gh` CLI not installed locally. Verify GitHub Actions run for commit `72ff0ca` passes (workflow now targets .NET 8.0.x and includes all 20 projects).
- [ ] **Container runtime** - Default connection string uses port 5432 (occupied by unrelated `unison-db-dev` on this machine). Dev/prod configs may need adjustment depending on environment.

## Authentication & Identity
- [ ] **Password hash mismatch** - Seeded bcrypt hash in `identity.users` (`$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4D.xZvy9ma`) does not verify against `Password123!` or any common variants. Auth harness uses env-based passwords (`DEMO_USER_PASSWORD || 'Password123!'`) and never reads `password_hash` from DB. Two sources of truth exist with no sync mechanism.
- [ ] **Production guard bypass** - Auth harness only throws if `NODE_ENV === 'production'` AND `DEMO_USER_PASSWORD` is unset. If `NODE_ENV` is not set, it silently runs with default credentials. Consider refusing to start in production outright or requiring explicit opt-in.
- [ ] **Issuer/PUBLIC_URL consistency in containers** - API fetches JWKS from `localhost:4000` by default; in Docker this will fail unless properly configured via environment variables.

## Database & Multi-Tenancy
- [ ] **Transaction-local scoping untested** - `TransactionStarted`/`SetTenantSessionAsync(..., isLocal: true)` compiles and is wired up, but no endpoint currently performs writes. The session-scoped path is proven via E2E, but the transaction-scoped path (critical for preventing leaks in pooled connections) has never been executed.
- [x] **Entity serialization leak risk** - Resolved via `[System.Text.Json.Serialization.JsonIgnore]` attribute on `DomainEvents` in `backend/School.Shared/Primitives/Entity.cs` AND dedicated `TenantResponse` DTO mapping in `backend/School.Api/Contracts/TenantResponse.cs` and `Program.cs`.
- [ ] **`platform.tenants` and `identity.roles` RLS** - Intentionally without RLS (root catalog/resolution and platform-wide roles). Any future "list tenants" endpoint must be super-admin gated in code.
- [ ] **Cross-tenant isolation** - Proven at DB level, but needs integration tests in CI (Testcontainers) as noted in testing strategy.

## Testing & Coverage
- [ ] **Integration tests missing** - Only unit tests (6) and architecture tests (2) exist. Need tests for interceptor lifecycle, query filters, RLS enforcement, and full auth flow.
- [ ] **Write-path tests** - Add tests that exercise `SaveChanges` within transactions to validate `is_local=true` session scoping.
- [ ] **Architecture tests incomplete** - Current tests only check entity inheritance and API dependency. Could add rules for tenant scoping, composite keys, etc.

## Modules & Solution
- [ ] **All 20 projects build individually** - Verified. The 5 untracked modules (Audit, Facilities, Grading, Health, Library) are now in solution and committed as csproj files. Ensure they have proper implementations or are intentional placeholders.
- [ ] **Solution structure drift** - Some modules may not be fully wired into DI or have implementations. Review if needed for Sprint 1 scope.

## Documentation
- [x] **Development plan progress** - Updated `docs/07-delivery/sprint-plan.md` and `docs/07-delivery/development-plan.md` from ~85% to ~75% to accurately represent current verification baseline.
- [ ] **Namespace clarification** - Note added to solution-structure.md that `School.*` is active while `FidelHub.*` is aspirational. Good, but other docs may still reference the old naming.

## Frontend
- [x] **Frontend dependencies & design tokens** - Verified `fidelTheme.ts` matches `#1B5E3F`, `#E0A526`, `#FAF7F0`, `#1F2933`. `package.json` lockfile integrity preserved. Zod and React Hook Form will be added cleanly alongside vertical slice form implementations.
