# WEB APPLICATION SPECIFICATION

## Fidel Hub — React + Vite Admin Portal

| Document Information | Details |
| :--- | :--- |
| **Document** | Web Application Specification |
| **Version** | 1.0-DRAFT |
| **Stack** | React 18+, Vite, TypeScript, Material UI (MUI) |

---

## 1. Application Architecture
- **Routing**: Client-side modular routing based on active domain features.
- **State Management**: TanStack React Query for server-state caching; Context API for session and active tenant.
- **Form Management**: React Hook Form + Zod / Yup validation mirroring backend FluentValidation rules.
- **Theme**: Custom Material UI theme (`fidelTheme.ts`) applying brand palette tokens.

---

## 2. Core Views & Components
- **Top Header**: Organization branding, tenant switcher, user profile, notifications.
- **Sidebar**: Core school navigation (Students, Academics, Attendance) + Specialized modules (Fees, Exams, Reports, Parent Updates).
- **Dashboard**: School overview statistics, tenant health checks, RLS status, offline sync queue monitors.
