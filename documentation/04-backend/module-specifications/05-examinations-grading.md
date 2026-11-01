# MODULE 05: EXAMINATIONS, CBT & GRADING SPECIFICATION

## Fidel Hub Backend Specification

| Module | Responsibility |
| :--- | :--- |
| **Domain Scope** | Continuous assessments, term examinations, offline Computer-Based Testing (CBT), grade boundaries, rankings, and report cards. |

---

## 1. Domain Entities & Value Objects
- `Assessment`: Quizzes, midterm tests, final examinations, weightings.
- `CbtExam`: Computer-based test package (questions, options, media, duration, rules).
- `CbtQuestion`: Multiple choice, true/false, fill-in-the-blank, point value.
- `CbtExamSession`: Student exam sitting (start time, submit time, client sync ID, answers payload).
- `Gradebook`: Teacher marksheets, term aggregates.
- `ReportCard`: Automated student term performance summary in Latin & Ethiopic scripts.

## 2. Key Commands & Queries
- `PublishCbtExamPackageCommand`: Generates encrypted downloadable assessment bundle for client offline taking.
- `SubmitCbtSessionCommand`: Ingests completed exam session from offline client with client UUID idempotency key.
- `CalculateStudentRanksCommand`: Computes classroom and branch rank percentiles.
- `BatchGenerateReportCardsCommand`: Compiles official PDF report cards with branch letterhead and verification QR.
