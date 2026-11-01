# DESIGN SYSTEM & BRAND TOKENS SPECIFICATION

## Fidel Hub — Visual Language, Colors, Typography & Design Tokens

| Document Information | Details |
| :--- | :--- |
| **Document ID** | DS-002 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Brand Colors** | Deep Green `#1B5E3F` · Warm Gold `#E0A526` · Dark Ink `#1F2933` · Off-White `#FAF7F0` |

---

## 1. Brand Color Palette & Semantic Design Tokens

### 1.1 Primary & Accent Brand Tokens
```css
:root {
  /* Primary Brand - Deep Forest Green */
  --fidel-primary-main: #1B5E3F;
  --fidel-primary-light: #2D8A5E;
  --fidel-primary-dark: #12422C;
  --fidel-primary-contrast: #FFFFFF;

  /* Secondary Accent - Warm Scholastic Gold */
  --fidel-secondary-main: #E0A526;
  --fidel-secondary-light: #EDB94C;
  --fidel-secondary-dark: #B88214;
  --fidel-secondary-contrast: #1F2933;

  /* Surfaces & Canvas Backgrounds */
  --fidel-bg-canvas: #FAF7F0;       /* Off-White / Cream background */
  --fidel-bg-surface: #FFFFFF;      /* Clean White Card/Table Surface */
  --fidel-bg-elevated: #FFFFFF;     /* Modals, Drawers, Dropdowns */
  --fidel-border: #E5E7EB;          /* Subtle Gray Dividers */
  --fidel-border-focused: #1B5E3F;  /* Form Input Focus Ring */

  /* Text & Typography */
  --fidel-text-primary: #1F2933;    /* Dark Ink */
  --fidel-text-secondary: #4B5563;  /* Subtitle & Secondary Label Gray */
  --fidel-text-muted: #9CA3AF;      /* Placeholder & Disabled text */

  /* Semantic Feedback States */
  --fidel-success: #1B5E3F;
  --fidel-success-bg: rgba(27, 94, 63, 0.10);
  --fidel-warning: #D97706;
  --fidel-warning-bg: rgba(217, 119, 6, 0.10);
  --fidel-error: #DC2626;
  --fidel-error-bg: rgba(220, 38, 38, 0.10);
  --fidel-info: #2563EB;
  --fidel-info-bg: rgba(37, 99, 235, 0.10);
}
```

---

## 2. Typography Hierarchy

### 2.1 Font Families
- **Latin Font**: `Inter, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif`
- **Ethiopic Font**: `"Noto Sans Ethiopic", "Abyssinica SIL", sans-serif`
- **Monospace Font**: `ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace`

### 2.2 Type Scale
| Level | Font Size | Line Height | Weight | Usage |
| :--- | :--- | :--- | :--- | :--- |
| **Display / H1** | `32px (2rem)` | `38px` | Bold (700) | Landing headers, login hero |
| **Page Title / H2** | `24px (1.5rem)` | `32px` | Bold (700) | Main page titles, dashboard headings |
| **Section Title / H3**| `18px (1.125rem)`| `26px` | SemiBold (600)| Card titles, drawer headers |
| **Subheading / H4** | `15px (0.9375rem)`| `22px` | SemiBold (600)| Table section headers, widget titles |
| **Body Primary** | `14px (0.875rem)` | `20px` | Regular (400) | Data table cells, form labels, general text |
| **Body Secondary** | `13px (0.8125rem)`| `18px` | Regular (400) | Descriptions, metadata, timestamps |
| **Caption / Badge** | `12px (0.75rem)` | `16px` | SemiBold (600)| Status badges, micro metrics, tooltips |

---

## 3. Brand Assets & SVG System

All vector assets are stored in `frontend/web/public/assets/` and `frontend/mobile/assets/`:

| Asset File | Usage in System |
| :--- | :--- |
| `fidel-hub-primary-logo.svg` | Desktop top header, login screen, print report card letterheads |
| `fidel-hub-symbol.svg` | Mobile app icon, browser favicon, collapsed sidebar icon |
| `fidel-hub-wordmark-white.svg` | Dark mode headers, presentation decks |
| `icon-exams-assessments.svg` | Question Bank, CBT Exam Runner, and assessments navigation |
| `icon-fees-payments.svg` | Fee structures, invoices, cashier terminal, and payment receipts |
| `icon-parent-updates.svg` | Communication module, SMS notifications, and guardian portal |
| `icon-report-cards.svg` | Term gradebook compilation, academic transcripts, and report card exports |

---

## 4. Spacing, Elevation & Border Radii

- **Border Radius**:
  - `sm`: `4px` (Tags, code blocks)
  - `md`: `8px` (Buttons, inputs, dropdown items)
  - `lg`: `12px` (Cards, data tables, dialogs)
  - `xl`: `16px` (Slide-over sheets, floating action bars)
  - `full`: `9999px` (Status badges, avatar circles)
- **Shadows & Elevation**:
  - Low (`card`): `0 1px 3px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.03)`
  - Medium (`dropdown`): `0 4px 6px -1px rgba(0, 0, 0, 0.08), 0 2px 4px -1px rgba(0, 0, 0, 0.04)`
  - High (`drawer/modal`): `0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)`
