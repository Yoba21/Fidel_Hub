# CODING STANDARDS & ENGINEERING GUIDELINES

## Fidel Hub — Engineering Quality Standards

| Document Information | Details |
| :--- | :--- |
| **Document** | Coding Standards & Guidelines |
| **Version** | 1.0-DRAFT |
| **Status** | Template / Specification Baseline |

---

## 1. General Principles
- **Maintain Documentation Integrity**: Preserve docstrings, comments, and architectural context.
- **Fail Fast & Explicitly**: Use `Result<T>` instead of throwing business exceptions for anticipated failures.
- **Async Throughout**: All I/O operations (`DbContext`, HTTP, Redis) must be purely asynchronous (`async/await`) with `CancellationToken` support.

---

## 2. C# / .NET Conventions
- **Language Version**: C# 12 / .NET 8+.
- **Nullable Reference Types**: `<Nullable>enable</Nullable>` is mandatory. No unhandled warnings.
- **Entities**: Private setters; state mutations must occur via explicit domain methods (e.g., `student.PromoteToGrade(...)`).
- **Dependency Injection**: Always inject interfaces (`ICurrentTenantContext`, `IMediator`), never concrete implementations.
- **Architecture Enforcement**: Automated `NetArchTest` runs on pull requests to ensure no module directly references another module's internal DbContext or Entities.

---

## 3. TypeScript / React Conventions
- **Strict Typing**: No `any` types. All props and API payloads must have explicit interfaces.
- **Component Organization**: Functional components with custom hooks for business logic.
- **Theme Usage**: No hardcoded color hex strings in components; always reference `theme.palette.primary.main`, `theme.palette.secondary.main`, etc.
- **Icons**: Use brand SVG icons from `/assets` for core domain modules and `lucide-react` for standard UI glyphs.
