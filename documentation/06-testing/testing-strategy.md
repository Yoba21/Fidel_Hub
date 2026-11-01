# TESTING STRATEGY SPECIFICATION

## Fidel Hub — Quality Assurance Framework

| Document Information | Details |
| :--- | :--- |
| **Document** | Testing Strategy Specification |
| **Version** | 1.0-DRAFT |

---

## 1. Test Pyramid & Automation Levels
1. **Architecture Tests (NetArchTest)**:
   - Automated enforcement verifying that domain modules do not illegally reference internal implementations of other modules.
2. **Unit Tests (xUnit + FluentAssertions + Moq)**:
   - High coverage on domain logic, double-entry balance calculations, and fee validation rules.
3. **Integration Tests (Testcontainers + PostgreSQL)**:
   - Real PostgreSQL test instance with RLS enabled to verify multi-tenant isolation policies and transaction rollbacks.
4. **End-to-End Tests (Playwright)**:
   - Cross-browser automation testing authentication, fee invoice settlement, and grade entry.
