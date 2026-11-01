# ENTITY FRAMEWORK CORE 10 MODELS SPECIFICATION

## Fidel Hub — Data Access & Domain Model Mapping

| Document Information | Details |
| :--- | :--- |
| **Document ID** | EFC-001 |
| **Version** | 1.0 |
| **Status** | Approved Baseline |
| **Framework** | Entity Framework Core 10 (Npgsql.EntityFrameworkCore.PostgreSQL) |
| **Pattern** | Domain-Driven Design (DDD) Aggregates, Value Objects & Fluent Configurations |

---

## 1. Domain Entity Foundations

Every persistent domain entity in Fidel Hub inherits from strongly-typed base abstractions ensuring multi-tenant isolation, branch scoping, and auditability.

```csharp
namespace FidelHub.Domain.Common;

public interface ITenantScoped
{
    Guid TenantId { get; }
}

public interface IBranchScoped : ITenantScoped
{
    Guid BranchId { get; }
}

public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; }
    Guid CreatedBy { get; }
    DateTime? UpdatedAtUtc { get; }
    Guid? UpdatedBy { get; }
}

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

---

## 2. Value Objects

### 2.1 Three-Part Ethiopian Naming Value Object
```csharp
public record EthiopianName(string FirstName, string FatherName, string GrandfatherName)
{
    public string FullName => $"{FirstName} {FatherName} {GrandfatherName}".Trim();
}
```

### 2.2 Monetary Value Object
```csharp
public record Money(decimal Amount, string Currency = "ETB")
{
    public static Money Zero(string currency = "ETB") => new(0.00m, currency);
    public Money Add(Money other) => other.Currency == Currency ? new(Amount + other.Amount, Currency) : throw new InvalidOperationException("Currency mismatch.");
}
```

---

## 3. Core Aggregate Models & Entity Definitions

### 3.1 Student Aggregate (`students` & `health` schemas)

```csharp
namespace FidelHub.Domain.Students;

public class Student : BaseEntity<Guid>, IBranchScoped, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid BranchId { get; private set; }
    public string AdmissionNumber { get; private set; } = default!;
    public EthiopianName Name { get; private set; } = default!;
    public DateOnly DateOfBirth { get; private set; }
    public string Gender { get; private set; } = default!;
    public string Status { get; private set; } = "ACTIVE"; // ACTIVE, TRANSFERRED, SUSPENDED, ALUMNI

    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Navigation properties
    public StudentHealthProfile? HealthProfile { get; private set; }
    public ICollection<StudentGuardian> Guardians { get; private set; } = new List<StudentGuardian>();

    private Student() { } // EF Core constructor

    public static Student Create(Guid tenantId, Guid branchId, string admissionNumber, EthiopianName name, DateOnly dob, string gender, Guid createdBy)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            BranchId = branchId,
            AdmissionNumber = admissionNumber,
            Name = name,
            DateOfBirth = dob,
            Gender = gender,
            Status = "ACTIVE",
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
```

### 3.2 CBT Examination Aggregate (`examinations` schema)

```csharp
namespace FidelHub.Domain.Examinations;

public class ExamPaper : BaseEntity<Guid>, IBranchScoped, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Title { get; private set; } = default!;
    public Guid SubjectId { get; private set; }
    public Guid AcademicTermId { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal TotalMarks { get; private set; }
    public decimal PassMarks { get; private set; }
    public bool ShuffleQuestions { get; private set; }
    public bool ShuffleOptions { get; private set; }
    public string Status { get; private set; } = "DRAFT"; // DRAFT, PUBLISHED, ARCHIVED

    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public ICollection<ExamQuestion> Questions { get; private set; } = new List<ExamQuestion>();
}

public class ExamSession : BaseEntity<Guid>, IBranchScoped
{
    public Guid TenantId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ExamPaperId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? SubmittedAtUtc { get; private set; }
    public int ElapsedSeconds { get; private set; }
    public bool IsFinal { get; private set; }
    public decimal? TotalScoreAwarded { get; private set; }
    public string Status { get; private set; } = "IN_PROGRESS"; // IN_PROGRESS, SUBMITTED, GRADED

    public ICollection<StudentAnswer> Answers { get; private set; } = new List<StudentAnswer>();
}
```

### 3.3 Student Fee & Invoicing Aggregate (`finance` schema)

```csharp
namespace FidelHub.Domain.Finance;

public class Invoice : BaseEntity<Guid>, IBranchScoped, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid BranchId { get; private set; }
    public string InvoiceNumber { get; private set; } = default!;
    public Guid StudentId { get; private set; }
    public Guid AcademicTermId { get; private set; }
    public DateOnly IssueDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public decimal SubtotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmountDue { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal BalanceDue => TotalAmountDue - PaidAmount;
    public string Status { get; private set; } = "DRAFT"; // DRAFT, POSTED, PARTIALLY_PAID, PAID, CANCELLED

    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public ICollection<InvoiceItem> Items { get; private set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    public void Post()
    {
        if (Status != "DRAFT") throw new InvalidOperationException("Only DRAFT invoices can be posted.");
        Status = "POSTED";
    }

    public void ApplyPayment(decimal amount)
    {
        if (Status != "POSTED" && Status != "PARTIALLY_PAID")
            throw new InvalidOperationException("Cannot apply payment to invoice in status: " + Status);
        
        PaidAmount += amount;
        Status = PaidAmount >= TotalAmountDue ? "PAID" : "PARTIALLY_PAID";
    }
}
```

### 3.4 Library Circulation Aggregate (`library` schema)

```csharp
namespace FidelHub.Domain.Library;

public class BookLoan : BaseEntity<Guid>, IBranchScoped
{
    public Guid TenantId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BookId { get; private set; }
    public string BorrowerType { get; private set; } = default!; // STUDENT, STAFF
    public Guid BorrowerId { get; private set; }
    public DateTime BorrowedAtUtc { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateTime? ReturnedAtUtc { get; private set; }
    public string? ReturnCondition { get; private set; } // GOOD, DAMAGED, LOST
    public string Status { get; private set; } = "ACTIVE"; // ACTIVE, RETURNED, OVERDUE, LOST
    public Guid CreatedBy { get; private set; }
}
```

---

## 4. Fluent API Configurations & Schema Mapping

```csharp
namespace FidelHub.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students", "students");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(s => s.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(s => s.AdmissionNumber).HasColumnName("admission_number").HasMaxLength(50).IsRequired();
        builder.HasIndex(s => new { s.TenantId, s.AdmissionNumber }).IsUnique();

        // Complex Type / Owned Entity for Ethiopian Name
        builder.OwnsOne(s => s.Name, nb =>
        {
            nb.Property(n => n.FirstName).HasColumnName("first_name").HasMaxLength(50).IsRequired();
            nb.Property(n => n.FatherName).HasColumnName("father_name").HasMaxLength(50).IsRequired();
            nb.Property(n => n.GrandfatherName).HasColumnName("grandfather_name").HasMaxLength(50).IsRequired();
        });

        builder.Property(s => s.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
        builder.Property(s => s.Gender).HasColumnName("gender").HasMaxLength(10).IsRequired();
        builder.Property(s => s.Status).HasColumnName("status").HasMaxLength(30).IsRequired();

        builder.Property(s => s.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(s => s.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");
    }
}
```

---

## 5. DbContext & Transactional Outbox SaveChanges Interceptor

```csharp
namespace FidelHub.Infrastructure.Data;

public class FidelHubDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public FidelHubDbContext(DbContextOptions<FidelHubDbContext> options, ITenantContext tenantContext) 
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FidelHubDbContext).Assembly);

        // Global Query Filter Defense-in-Depth for Multi-Tenancy
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            {
                var method = SetTenantFilterMethod.MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Audit stamp assignment
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAtUtc").CurrentValue = DateTime.UtcNow;
                entry.Property("CreatedBy").CurrentValue = _tenantContext.UserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAtUtc").CurrentValue = DateTime.UtcNow;
                entry.Property("UpdatedBy").CurrentValue = _tenantContext.UserId;
            }
        }

        // 2. Dispatch domain events to Transactional Outbox
        var domainEntities = ChangeTracker.Entries<BaseEntity<Guid>>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        foreach (var entity in domainEntities)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                var outboxMessage = new OutboxMessage(
                    Guid.NewGuid(),
                    _tenantContext.TenantId,
                    domainEvent.GetType().Name,
                    JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    DateTime.UtcNow
                );
                Set<OutboxMessage>().Add(outboxMessage);
            }
            entity.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```
