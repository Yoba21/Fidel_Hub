# AUTHENTICATION & AUTHORIZATION SPECIFICATION

## Fidel Hub — Security Token & Claims Architecture

| Document Information | Details |
| :--- | :--- |
| **Document** | Authentication & Authorization Specification |
| **Version** | 1.0-DRAFT |

---

## 1. Authentication Flow
- **Mechanism**: JSON Web Tokens (JWT) issued via ASP.NET Core Identity.
- **Access Token Lifetime**: 15 minutes.
- **Refresh Token Lifetime**: 14 days (stored in HTTP-only, Secure cookie with rotation).

---

## 2. JWT Claims Schema
```json
{
  "sub": "4a73e86c-...",
  "email": "principal@stjoseph.edu.et",
  "tenant_id": "c4b8b6e2-...",
  "branch_id": "19b52a10-...",
  "roles": ["SchoolPrincipal", "AcademicDirector"],
  "exp": 1793524800
}
```

---

## 3. Branch-Level Authorization
In addition to tenant scoping, operations on students, attendance, or finance can optionally be restricted to specific campus branches (`branch_id`).
