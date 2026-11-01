# Fidel Hub — Better Auth Identity Microservice

## Overview
Lightweight dedicated authentication microservice for Fidel Hub providing:
- Multi-Tenant Organization and Branch context resolution.
- RS256/EdDSA cryptographically signed JWT issuance.
- Standard RFC 7517 `/.well-known/jwks.json` endpoint consumed by ASP.NET Core `JwtBearer`.

## Endpoints
- `GET /health`: Health check probe.
- `GET /.well-known/jwks.json`: Public keys for JWT validation.
- `POST /api/auth/sign-in`: Authenticate user and issue scoped bearer token with `tenant_id`, `branch_id`, `roles`, and `permissions`.

## Running Locally
```bash
npm install
npm run dev
```
Runs by default on `http://localhost:4000`.
