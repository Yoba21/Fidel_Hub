# BACKEND ARCHITECTURE SPECIFICATION

## Fidel Hub — ASP.NET Core Modular Monolith

| Document Information | Details |
| :--- | :--- |
| **Document** | Backend Architecture Specification |
| **Version** | 1.0-DRAFT |
| **Framework** | ASP.NET Core 8+ / .NET 10 |

---

## 1. Architectural Layers

Each domain module encapsulates:
1. **Domain Layer**: Core business logic, domain entities, value objects, and domain events. Completely decoupled from external dependencies or persistence libraries.
2. **Application Layer**: Use cases, CQRS commands & queries, FluentValidation validators, and event handlers.
3. **Infrastructure Layer**: Entity Framework Core persistence configurations, repositories, Dapper performance queries, and external service gateways.
4. **Presentation Layer**: Thin ASP.NET Core minimal API endpoints or controllers registered into the main `School.Api` host.

---

## 2. In-Process & Asynchronous Communication
- **Synchronous In-Process**: Public interface contracts (e.g., `IStudentModuleApi`, `IFinanceModuleApi`) registered in Dependency Injection. Cross-module database queries are forbidden.
- **Asynchronous Domain Events**: Events written to `outbox_messages` table within the same database transaction. The `School.Worker` Quartz background engine processes and dispatches events.
