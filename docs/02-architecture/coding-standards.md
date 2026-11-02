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
- **Language Level**: C# 12 / .NET 8 LTS (`net8.0`).
- **Nullable Reference Types**: Mandatory `<Nullable>enable</Nullable>`. Code must compile with zero unhandled null warnings.
- **Architecture Enforcement**: Automated NetArchTest rules verify that domain entities do not depend on external infrastructure or presentation layers.
- **Database Access**: EF Core for command mutations and relationship tracking; Dapper for high-volume paginated read queries and complex reports.

---

## 3. TypeScript & React Standards
- **Framework Baseline**: React 18 + Vite 5 + TypeScript 5 (Strict Mode enabled).
- **Strict Typing**: No `any` or loose types. All props, API requests, and response payloads must have explicit TypeScript interfaces.
- **Design Tokens**: Never hardcode raw color hex strings in components. Reference Material UI (MUI v6) theme tokens via the `fidelTheme` palette (`theme.palette.primary.main` #1B5E3F, `theme.palette.secondary.main` #E0A526, `theme.palette.background.default` #FAF7F0, `theme.palette.text.primary` #1F2933, `theme.palette.success.main` #2E7D32, `theme.palette.error.main` #D32F2F) or CSS custom properties.
- **State Architecture**: Use `@tanstack/react-query` (v5) for server state caching (5-minute `staleTime`, optimistic updates) and `zustand` for lightweight client UI state (`useAuthStore`, `useBranchStore`, `useUIStore`).
- **Data Tables**: Table views must provide empty state fallbacks, loading skeletons, and localized dates (supporting both Gregorian and Ethiopian formats).
- **SQLite WAL**: Local venue server CBT operations (and mobile read caching) must configure `PRAGMA journal_mode = WAL;` and wrap bulk question/answer insertions in local transactions. Offline write queueing is strictly confined to CBT in the initial release.

---

## 4. Ecosystem & Approved Libraries Standards

Fidel Hub web frontend exclusively standardizes on 10 approved ecosystem packages:

### 4.1 Schema Validation with Zod (`zod`)
- **Colocated Form Schemas**: Define validation schemas adjacent to the feature component (e.g. `student-registration.schema.ts`).
- **Infer TypeScript Types**: Always derive form types directly from the schema using `z.infer<typeof schema>` to maintain a single source of truth.
- **Pre-Submit Validation**: Pair Zod schemas with `react-hook-form` via `@hookform/resolvers/zod` to prevent invalid requests from hitting the network.

### 4.2 Form State Management with React Hook Form (`react-hook-form`)
- **Uncontrolled Inputs**: Use React Hook Form with uncontrolled inputs and ref forwarding for zero-re-render high-performance form entries.
- **Dynamic Field Arrays**: Use `useFieldArray` for dynamic line-item entry in marksheet grades and invoice fee breakdowns.

### 4.3 UI Primitives with shadcn/ui & Radix UI
- **Component Ownership**: Components reside inside `src/components/ui/` and are customized to conform to the `fidelTheme` token design system.
- **Accessible Interactions**: Ensure all interactive elements (modals, dropdowns, command palettes) support full keyboard accessibility (`Tab`, `Escape`, `Enter`, arrow navigation) and screen-reader ARIA attributes.
- **Command Palette (`Ctrl+K`) (`cmdk`)**: Use `cmdk` for fast keyboard-driven switching between students, branches, and quick action shortcuts.

### 4.4 Iconography with Tabler Icons (`@tabler/icons-react`)
- **Direct Named Imports**: Always import icons individually (e.g., `import { IconSchool, IconReceipt } from '@tabler/icons-react'`) to enable effective tree-shaking.
- **Standard Sizing**: Standardize icon sizes: `18px` for dense table actions and badges, `20px` for navigation links, `24px` for page headers.

### 4.5 Server State & Caching with TanStack Query (`@tanstack/react-query`)
- **Cache Invalidation**: Configure 5-minute default `staleTime`; invalidate active query keys upon successful mutations.
- **Optimistic Updates**: Apply optimistic mutations for rapid marksheet grade entries and daily attendance rosters with rollback on failure.

### 4.6 Client UI State with Zustand (`zustand`)
- **Minimalist Stores**: Keep client UI stores focused and decoupled (`useAuthStore`, `useBranchStore`, `useUIStore`). Avoid storing server data in Zustand.

### 4.7 Date & Time Manipulation with Day.js (`dayjs`)
- **Modular Plugins**: Only import necessary Day.js plugins (`relativeTime`, `utc`, `duration`) to keep client bundle size minimal (< 5KB).
- **Timezone Invariant**: Store and transmit timestamps strictly in UTC (`ISO 8601`). Format to East Africa Time (EAT / UTC+3) only in presentation components.

### 4.8 Utility Functions with Lodash-es (`lodash-es`)
- **Tree-Shakeable Syntax**: Never use default imports (`import _ from 'lodash'`). Use named ES imports: `import { debounce, throttle, groupBy, cloneDeep } from 'lodash-es'`.
- **Debounced Search**: Apply a 300ms debounce on text inputs in the student directory and command palette.
- **Throttled Autosave**: Throttle client-side CBT autosave state to a maximum of 1 write per 500ms to avoid saturating storage during rapid typing.

### 4.9 Telemetry & Error Tracking with Sentry (`@sentry/react`)
- **Contextual Breadcrumbs**: Attach user role, active `tenant_id`, and active `branch_id` (never passwords or national ID numbers) as Sentry context tags.
- **CBT Error Boundaries**: Wrap the student test-taking interface in dedicated React Error Boundaries that log fatal exceptions to Sentry while preserving local SQLite answer state.

### 4.10 AI Workflows with Vercel AI SDK (`ai`)
- **Streaming Responses**: Utilize Vercel AI SDK client hooks (`useChat`, `useCompletion`) for streaming rubric evaluation and curriculum Question generation.
