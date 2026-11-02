# Fidel Hub — Better Auth Identity Microservice (Sprint 1 Test-Double)

> [!NOTE]
> This service acts as the **Sprint 1 local test-double / mock harness** for Better Auth. It implements RFC 7517 (`/.well-known/jwks.json`) and OpenID Connect discovery (`/.well-known/openid-configuration`) with RS256 JWT minting to validate ASP.NET Core asymmetric token verification during development. The full production Better Auth implementation (with PostgreSQL persistence, OAuth providers, and session management) will replace this double during staging deployment.

## Overview
Dedicated authentication microservice harness providing:
- Multi-Tenant Organization and Branch context resolution.
- RS256 cryptographically signed JWT issuance.
- Standard RFC 7517 `/.well-known/jwks.json` and OpenID Connect discovery endpoints consumed by ASP.NET Core `JwtBearer`.

## Endpoints
- `GET /health`: Health check probe.
- `GET /.well-known/openid-configuration`: OIDC discovery metadata.
- `GET /.well-known/jwks.json`: Public keys for JWT validation.
- `POST /api/auth/sign-in`: Authenticate user and issue scoped bearer token with `tenant_id`, `branch_id`, `roles`, and `permissions`.

## Authentication Credentials & Data Model
- **Sprint 1 Local Test-Double**: Validates credentials against environment configuration (`DEMO_USER_EMAIL`, default: `admin@fidelhub.academy` / `DEMO_USER_PASSWORD`, default: `Password123!`).
- **Database Schema**: `identity.users.password_hash` contains a standard bcrypt hash reserved for the persistent database authentication driver in future sprints. During Sprint 1, the test-double harness mints tokens in-memory without directly querying the database credentials table.

## Running Locally
```bash
npm install
npm run dev
```
Runs by default on `http://localhost:4000`.
