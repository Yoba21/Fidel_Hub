# ADR-001: Modular Monolith Architecture over Microservices

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |
| **Consulted** | Engineering, Operations |

---

## 1. Context & Problem Statement
Fidel Hub requires a high-performance, robust platform capable of handling student lifecycle, continuous assessment grading, offline CBT synchronization, and fee invoicing. We evaluated whether to deploy independent microservices, serverless functions, or a Domain-Driven Modular Monolith in ASP.NET Core.

## 2. Decision Outcome
Chosen: **Domain-Driven Modular Monolith in ASP.NET Core**.

### Rationale:
- A single deployable containerized API eliminates distributed transaction failures and network serialization latency across closely related school operations.
- Each module (`Academics`, `Students`, `Examinations`, `Finance`, `Attendance`, `Identity`) maintains strict internal separation (`Domain`, `Application`, `Infrastructure`) with explicit public interface contracts.
- Automated architecture tests (`School.ArchitectureTests` via NetArchTest) fail CI/CD if one module directly references the internal DbContext or Entities of another module.
- Retains the organizational benefits of microservices without the infrastructure overhead of managing Kubernetes clusters for school venues.
