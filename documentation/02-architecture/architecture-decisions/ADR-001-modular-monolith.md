# ADR-001: Modular Monolith Architecture over Microservices

| Metadata | Value |
| :--- | :--- |
| **Status** | Accepted |
| **Date** | 2026-11-01 |
| **Deciders** | Architecture Working Group |
| **Consulted** | Engineering, Operations |

---

## 1. Context & Problem Statement
Fidel Hub requires a system capable of managing multiple high-throughput educational workflows (grading, attendance, student lifecycle, and finance). The choice is between:
1. Microservices architecture (deploying independent microservices per domain).
2. Distributed serverless functions.
3. Modular Monolith (a unified deployable artifact internally structured into strictly isolated domain modules).

## 2. Decision Drivers
- Operational complexity of running K-8s/Docker microservices for Ethiopian schools is prohibitive.
- Need for transactional consistency across student enrollments and fee generation.
- Clear business boundaries are essential so teams can work in parallel without coupling.
- Low DevOps maintenance footprint.

## 3. Considered Options
- Option 1: Full Microservices (11 services: Identity, Students, Finance, etc.).
- Option 2: Monolithic Layered N-Tier Application (Traditional Controller-Service-Repository).
- Option 3: Domain-Driven Modular Monolith in ASP.NET Core.

## 4. Decision Outcome
Chosen **Option 3: Domain-Driven Modular Monolith**.

### Positive Consequences
- Single unified deployment unit (one ASP.NET Core process + Worker service).
- In-process communication with strong typing, zero network serialization latency.
- Strict module encapsulation prevents "spaghetti" dependencies via NetArchTest rules.
- Path to extract a high-load module (e.g. Attendance or Comms) into an independent service later if ever needed.

### Negative Consequences / Trade-offs
- Teams must respect strict module boundaries without taking shortcuts such as raw cross-module database joins.
- Shared deployment means all modules deploy simultaneously.

## 5. Compliance Rules
- Direct database references between modules are strictly forbidden.
- Synchronous communication must use exposed public contracts (`I<Module>Api`).
- Asynchronous communication must use the Transactional Outbox.
