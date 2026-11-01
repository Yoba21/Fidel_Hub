# USER ROLES & PERMISSIONS MATRIX

## Fidel Hub — Access Control & Authorization Framework

| Document Information | Details |
| :--- | :--- |
| **Document** | User Roles & Permissions Matrix |
| **Version** | 1.0-DRAFT |
| **Status** | Template / Specification Baseline |
| **Scope** | Platform, Tenant, and Branch Authorization |

---

## 1. Role Hierarchy

```
+-------------------------------------------------------+
|                 Platform SuperAdmin                  |
+-------------------------------------------------------+
                           |
+-------------------------------------------------------+
|               Tenant Director / Principal            |
+-------------------------------------------------------+
                           |
+-------------------------------------------------------+
|               Branch Campus Administrator             |
+-------------------------------------------------------+
        |                  |                  |
+---------------+  +---------------+  +---------------+
|    Teacher    |  |  Finance/Cash |  |  Registrar/HR |
+---------------+  +---------------+  +---------------+
        |                  |
+---------------+  +---------------+
| Parent/Guard. |  |    Student    |
+---------------+  +---------------+
```

---

## 2. Granular Permissions Catalog

### 2.1 Tenant & Organization (`tenants:*`)
- `tenants.view`: View organization details and settings.
- `tenants.edit`: Update organization profile, logo, and contacts.
- `branches.manage`: Create and configure campuses/branches.
- `subscriptions.view`: View subscription tiers and billing invoices.

### 2.2 Student Lifecycle (`students:*`)
- `students.view`: View student directories and profiles.
- `students.create`: Register new student admissions.
- `students.edit`: Update student medical, guardian, or demographic records.
- `students.archive`: Archive, graduate, or withdraw student profiles.

### 2.3 Academics & Classes (`academics:*`)
- `academics.calendar.manage`: Configure Ethiopian & Gregorian term schedules.
- `academics.curriculum.manage`: Setup subjects, departments, and course syllabi.
- `academics.classes.assign`: Assign teachers and students to sections.

### 2.4 Attendance (`attendance:*`)
- `attendance.take`: Mark homeroom or period-level daily attendance.
- `attendance.modify`: Revise historical attendance entries with audit note.
- `attendance.reports.view`: View attendance summaries and truancy alerts.

### 2.5 Examinations & Grading (`exams:*`)
- `exams.schedule`: Setup assessment dates, test weights, and scorecards.
- `exams.grades.enter`: Enter classroom assessment scores and exam marks.
- `exams.grades.lock`: Finalize and lock term gradebooks.
- `exams.reports.publish`: Publish student report cards and transcripts.

### 2.6 Finance & Ledger (`finance:*`)
- `finance.fees.configure`: Setup fee categories, amounts, and installment plans.
- `finance.invoices.generate`: Batch generate student term fee invoices.
- `finance.payments.record`: Receive payments, issue receipts, and record bank slips.
- `finance.ledger.view`: View double-entry journal vouchers and account balances.
- `finance.ledger.reverse`: Post reversing journal adjustment vouchers.

---

## 3. Permissions Matrix by Role

| Permission Area | SuperAdmin | School Principal | Branch Admin | Teacher | Finance Officer | Parent | Student |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| `tenants.manage` | **Full** | View/Edit | None | None | None | None | None |
| `branches.manage` | **Full** | **Full** | Scope Only | None | None | None | None |
| `students.view` | All | All Branches | Branch Only | Class Only | Branch Only | Own Children | Self |
| `students.create` | Yes | Yes | Yes | No | No | No | No |
| `attendance.take` | Yes | Yes | Yes | Assigned Class | No | No | No |
| `exams.grades.enter`| Yes | Yes | Yes | Assigned Subj | No | No | No |
| `exams.grades.lock` | Yes | Yes | Branch Only | No | No | No | No |
| `finance.payments` | No | View Only | View Only | No | **Full** | Pay Only | View Only |
| `finance.ledger` | No | View Only | View Only | No | **Full** | No | No |

---

## 4. Multi-Tenant Authorization Enforcement

1. **Token Claims**:
   - `sub`: User UUID
   - `tenant_id`: Current active tenant UUID
   - `branch_id`: Specific campus assignment (optional or array)
   - `roles`: `["Teacher", "BranchAdmin"]`
   - `permissions`: Compressed Bitmask or claim list
2. **Policy Enforcement Middleware**:
   - Controller/Endpoint decorated with `[Authorize(Policy = "Permissions.Finance.Payments.Record")]`.
   - Validates that user has permission AND that target resource belongs to the session `tenant_id`.
