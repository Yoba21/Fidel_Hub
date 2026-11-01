# ENTERPRISE UI / UX SPECIFICATION

## Fidel Hub — Design Language, Component Anatomy & Enterprise UX Patterns

| Document Information | Details |
| :--- | :--- |
| **Document ID** | UX-001 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Design Paradigm** | High-Density Enterprise Workspaces (Adapted from Modern Institutional Systems) |
| **Target Clients** | Web Admin Portal (Desktop/Laptop), Mobile & Tablet Client (React Native + Expo), and Offline CBT Lab Workstations |

---

## 1. Enterprise UX Philosophy & Design Pillars

Fidel Hub is engineered for school principals, branch directors, registrars, and cashiers who interact with student data for hours daily. The interface prioritizes **clarity, speed, and context preservation** over decorative fluff:

1. **Context-Preserving Navigation**:
   - Use slide-over detail drawers (Sheets) and contextual modals for viewing profiles, entering grades, or recording fee payments, ensuring administrators never lose their filtered table position.
2. **High Information Density with Visual Breathing Room**:
   - Compact table rows (40–48px), clean monospace identifiers for admission numbers and voucher IDs, and scannable visual status badges.
3. **Fidel Hub Visual Identity**:
   - **Deep Green (`#1B5E3F`)**: Institutional trust, header surfaces, primary action triggers.
   - **Warm Gold (`#E0A526`)**: Badges, scholastic achievements, active tab highlights, and alerts.
   - **Dark Ink (`#1F2933`)**: High-contrast, fatigue-free typography.
   - **Warm Off-White (`#FAF7F0`)**: Warm canvas background reducing glare compared to stark pure white.
   - **Dual Typography**: `Inter` for Latin characters paired with `Noto Sans Ethiopic` / `Abyssinica SIL` for seamless Amharic and Ge'ez (ፊ) text rendering.
4. **Resilient Feedback & Offline Transparency**:
   - Persistent connectivity pill in the global header: **Online (Green)** or **Offline Mode (Amber)** displaying unsynced operation counts and manual sync controls.

---

## 2. Standard Enterprise Page Anatomy

Every domain view across Fidel Hub (Academics, Students, Examinations, Fees, Attendance) strictly follows a unified 5-tier page layout:

```text
+----------------------------------------------------------------------------------------------------+
| 1. GLOBAL APP HEADER                                                                               |
|    [ፊ Fidel Hub Logo]  [Tenant: St. Joseph Academy ▼]  [Branch: Kera Campus ▼]  [Search (Ctrl+K)]  |
|                                                     [Offline Sync Pill: 🟢 Online]  [🔔 (3)]  [Avatar] |
+----------------------------------------------------------------------------------------------------+
| 2. PAGE HEADER & BREADCRUMBS                                                                       |
|    Home > Academics > Grade 9 Chemistry                                                            |
|    Grade 9 Chemistry — Section A & B                                                               |
|    Academic Year 2026/27 (Term 1) · Teacher: Mr. Abebe Tessema                                      |
|    [Filter Range]  [Export CSV]  [+ Add Assessment / New Entry (Primary CTA)]                     |
+----------------------------------------------------------------------------------------------------+
| 3. KPI SUMMARY STATS CARDS (Top Banner)                                                            |
|    +-------------------+  +-------------------+  +-------------------+  +-------------------+      |
|    | Enrolled Students |  | Attendance Today  |  | Tuition Collected |  | Avg Exam Score    |      |
|    | 84 Students       |  | 98.2%             |  | ETB 342,000       |  | 74.5% (B+)        |      |
|    | +2 transfers      |  | 82 Present, 2 Abs |  | 92% of billed     |  | +4.2% vs Term 1   |      |
|    +-------------------+  +-------------------+  +-------------------+  +-------------------+      |
+----------------------------------------------------------------------------------------------------+
| 4. TABBED DOMAIN NAVIGATION                                                                        |
|    [Overview]  [Roster / Students]  [Timetable]  [Continuous Assessment]  [Gradebook]  [Settings]   |
+----------------------------------------------------------------------------------------------------+
| 5. DATA WORKSPACE (Filter Toolbar + High-Density Table / Kanban / CBT Runner)                       |
|    [Search student name/ID...] [Filter: Gender ▼] [Filter: Status ▼]        [Bulk Actions (0) ▼]   |
|    +-----+--------------------+--------------------+-----------+--------------------+----------+   |
|    | [ ] | Admission No.      | Student Name       | Status    | Fee Status         | Actions  |   |
|    +-----+--------------------+--------------------+-----------+--------------------+----------+   |
|    | [ ] | SJA-2026-0042      | Almaz Hailu Bekele | Active    | Paid (Receipt #42) | [...]    |   |
|    | [ ] | SJA-2026-0043      | Dawit Yohannes     | Active    | Partial (ETB 500)  | [...]    |   |
|    +-----+--------------------+--------------------+-----------+--------------------+----------+   |
|    Showing 1-25 of 84 records                                          [ < 1 2 3 4 > ]             |
+----------------------------------------------------------------------------------------------------+
```

---

## 3. Core Component Library & Patterns (Adapted from Enterprise Modules)

### 3.1 Status Badges & Pills
Status indicators are standardized with subtle tinted backgrounds and high-contrast text:

| Semantic State | Background | Text Color | Usage in Fidel Hub |
| :--- | :--- | :--- | :--- |
| **Success / Paid** | `rgba(27, 94, 63, 0.12)` | `#1B5E3F` | `Paid`, `Active`, `Present`, `Passed`, `Synced` |
| **Warning / Partial** | `rgba(224, 165, 38, 0.15)` | `#B88214` | `Partial Payment`, `Late`, `Under Review`, `Pending Sync` |
| **Danger / Truant** | `rgba(220, 38, 38, 0.12)` | `#DC2626` | `Unexcused Absent`, `Overdue`, `Failed`, `Withdrawn` |
| **Neutral / Draft** | `rgba(107, 114, 128, 0.12)` | `#4B5563` | `Draft`, `Scheduled`, `Upcoming`, `Offline Queue` |

### 3.2 High-Density Data Tables
- **Selection & Bulk Action Bar**: Selecting checkboxes reveals a floating action banner (`[2 Selected] -> [Send SMS Alert] [Generate Invoices] [Export PDF]`).
- **Sticky Column Headers**: Table headers remain visible during vertical scrolling with instant alphanumeric sorting arrows.
- **Monospace Identifiers**: Admission numbers, transaction vouchers, and receipts render in clean monospace font (`ui-monospace, SFMono-Regular, Menlo, monospace`) for zero-ambiguity character scanning.
- **Contextual Action Menus**: Row-level `MoreVertical` ellipsis menus containing role-permitted actions (`View Details`, `Edit Record`, `Print Receipt`, `Send Notice`).

### 3.3 Slide-Over Detail Drawers (Sheet Pattern)
- Clicking a student, teacher, or invoice row opens an 800px slide-over sheet from the right side of the screen.
- Displays full biodata, fee payment history tabs, guardian contact shortcuts (one-click `Call` or `SMS`), and health notes without navigating away from the search list.

### 3.4 Form Layouts & Inline Validation
- Dual-column responsive forms with floating labels and clear field groupings (Personal Info, Academic Assignment, Guardian Details).
- Real-time client validation mirroring backend FluentValidation rules.
- Instant format masking for Ethiopian phone numbers (`+251 9XX XXX XXX` or `09XX XXX XXX`).

---

## 4. Specialized Module UX Workflows

### 4.1 Examination & CBT Test Runner UI (The Flagship Experience)
- **Exam Lobby**: Candidate verification, rules overview, system compatibility check, and encrypted package integrity confirmation.
- **Full-Screen Exam Environment**:
  - Top Bar: Exam Title, Subject, Current Candidate Name, and prominent remaining countdown timer with amber/red urgency transitions.
  - Left/Bottom Question Grid: Interactive 1-to-$N$ question grid showing visual states (`Current`, `Answered`, `Unanswered`, `Flagged for Review`).
  - Question Viewport: High-contrast typography with LaTeX/KaTeX math formula rendering and responsive image magnification.
  - Option Selection: Large touch-friendly radio pills with instant keyboard shortcuts (`A`, `B`, `C`, `D` or `1`, `2`, `3`, `4`).
  - Autosave Status Pill: Micro-indicator in the corner continuously showing `Saved to Local Database (WAL)` with zero lag.
  - Submission Modal: Summary review dialog (`You have answered 48 of 50 questions; 2 are flagged`) before final cryptographic submission.

### 4.2 Gradebook & Continuous Assessment Marksheet
- Excel-like continuous spreadsheet interface with keyboard arrow-key navigation.
- Automatic live computation of term weighted totals as teachers type in quiz or test marks.
- Out-of-bounds warning (e.g. typing `25` in a test weighted out of `20` flashes red with inline error).

### 4.3 Student Fee Collection & Cashier Terminal
- Quick-search by student name, admission number, or parent phone number.
- Instant invoice balance card showing total term fees, payments made, and remaining balance.
- Payment entry with quick denomination buttons (Full Payment, 50% Installment).
- One-click receipt generation with instant browser print dialog and automated confirmation SMS.

---

## 5. Responsive & Accessibility Standards (WCAG 2.1 AA)
- **Contrast Ratio**: Minimum 4.5:1 contrast for all text against backgrounds (surpassed by Dark Ink `#1F2933` on `#FAF7F0` with 13.8:1 contrast).
- **Keyboard Operability**: Full keyboard navigation across data tables, modals, and the CBT test runner.
- **Touch Targets**: Minimum 44x44px touch targets on mobile and tablet interfaces.
