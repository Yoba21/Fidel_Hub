# CODING STANDARDS & ENGINEERING GUIDELINES

## Fidel Hub — Engineering Quality Standards

| Document Information | Details |
| :--- | :--- |
| **Document ID** | STD-001 |
| **Version** | 1.1 |
| **Status** | Approved Baseline |

---

## 1. General Engineering Principles
- **Explicit Domain Rules**: Business invariants belong in rich domain entities with private setters, not anemic data models with public properties.
- **Fail Fast & Explicitly**: Use `Result<T>` rather than throwing business exceptions for anticipated validation or domain rejections.
- **Purely Asynchronous I/O**: All database, HTTP, and disk I/O operations must be fully asynchronous (`async/await`) with `CancellationToken` cascading.

---

## 2. C# / .NET Standards
- **Language Level**: C# 12 / .NET 8 / 10 LTS.
- **Nullable Reference Types**: Mandatory `<Nullable>enable</Nullable>`. Code must compile with zero unhandled null warnings.
- **Architecture Enforcement**: Automated NetArchTest rules verify that domain entities do not depend on external infrastructure or presentation layers.
- **Database Access**: EF Core for command mutations and relationship tracking; Dapper for high-volume paginated read queries and complex reports.

---

## 3. TypeScript & React Standards
- **Strict Typing**: No `any` or loose types. All props, API requests, and response payloads must have explicit TypeScript interfaces.
- **Design Tokens**: Never hardcode raw color hex strings in components. Reference `theme.palette.primary.main`, `theme.palette.secondary.main`, or CSS custom properties.
- **Data Tables**: Table views must provide empty state fallbacks, loading skeletons, and localized dates (supporting both Gregorian and Ethiopian formats).
- **SQLite WAL**: Mobile and local venue server operations must configure `PRAGMA journal_mode = WAL;` and wrap bulk question/answer insertions in local transactions.

---

## 4. Ecosystem & Approved Libraries Standards

### 4.1 Schema Validation with Zod
- **Colocated Form Schemas**: Define validation schemas adjacent to the feature component (e.g. `student-registration.schema.ts`).
- **Infer TypeScript Types**: Always derive form types directly from the schema using `z.infer<typeof schema>` to maintain a single source of truth.
- **Pre-Submit Validation**: Pair Zod schemas with `react-hook-form` via `@hookform/resolvers/zod` to prevent invalid requests from hitting the network.

### 4.2 UI Primitives with shadcn/ui & Radix UI
- **Component Ownership**: Components reside inside `src/components/ui/` and can be customized per Fidel Hub branding.
- **Accessible Interactions**: Ensure all interactive elements (modals, dropdowns, command palettes) support full keyboard accessibility (`Tab`, `Escape`, `Enter`, arrow navigation) and screen-reader ARIA attributes.
- **Command Palette (`Ctrl+K`)**: Use `cmdk` for fast keyboard-driven switching between students, branches, and quick action shortcuts.

### 4.3 Iconography with Tabler Icons
- **Direct Named Imports**: Always import icons individually (e.g., `import { IconSchool, IconReceipt } from '@tabler/icons-react'`) to enable effective tree-shaking.
- **Standard Sizing**: Standardize icon sizes: `18px` for dense table actions and badges, `20px` for navigation links, `24px` for page headers.

### 4.4 Date & Time Manipulation with Day.js
- **Modular Plugins**: Only import necessary Day.js plugins (`relativeTime`, `utc`, `duration`) to keep client bundle size minimal (< 5KB).
- **Timezone Invariant**: Store and transmit timestamps strictly in UTC (`ISO 8601`). Format to East Africa Time (EAT / UTC+3) only in presentation components.

### 4.5 Utility Functions with Lodash-es
- **Tree-Shakeable Syntax**: Never use default imports (`import _ from 'lodash'`). Use named ES imports: `import { debounce, throttle, groupBy, cloneDeep } from 'lodash-es'`.
- **Debounced Search**: Apply a 300ms debounce on text inputs in the student directory and command palette.
- **Throttled Autosave**: Throttle client-side CBT autosave state to a maximum of 1 write per 500ms to avoid saturating storage during rapid typing.

### 4.6 Telemetry & Error Tracking with Sentry
- **Contextual Breadcrumbs**: Attach user role, active `tenant_id`, and active `branch_id` (never passwords or national ID numbers) as Sentry context tags.
- **CBT Error Boundaries**: Wrap the student test-taking interface in dedicated React Error Boundaries that log fatal exceptions to Sentry while preserving local SQLite answer state.
