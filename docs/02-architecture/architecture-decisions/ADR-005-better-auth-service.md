# ADR-005: Authentication Architecture via Dedicated Better Auth Service

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |

---

## 1. Context & Problem Statement
Fidel Hub requires robust multi-tenant authentication, organization switching, session management, secure refresh token rotation, and multi-factor authentication (2FA) across React (Vite) web applications and React Native (Expo) mobile apps. We evaluated native ASP.NET Core Identity vs. a dedicated **Better Auth** authentication service.

## 2. Decision Outcome
Chosen: **Dedicated Better Auth Service (Node.js/PostgreSQL) with Organizations & JWT Plugins + ASP.NET Core `JwtBearer` Validation**.

### Architecture & Protocol:
1. **Dedicated Auth Container**: A lightweight service running Better Auth connected to the shared PostgreSQL database.
2. **Organizations Plugin**: Maps directly to school tenants (`tenant_id`), managing memberships, invitations, and active tenant contexts.
3. **JWT Plugin & Claims**: Issues signed JSON Web Tokens containing `sub`, `tenant_id`, `branch_id`, and `roles`.
4. **Client DX**: React and Expo clients utilize official `@better-auth/client` and `@better-auth/react` hooks for instantaneous logins, 2FA, and session listeners.
5. **ASP.NET Core Verification**: Core modular monolith verifies tokens via standard `Microsoft.AspNetCore.Authentication.JwtBearer` against Better Auth's public JWKS endpoint, extracting `tenant_id` to set `ICurrentTenantContext` for PostgreSQL RLS.
