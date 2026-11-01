using FluentAssertions;
using School.Shared.Entities;
using Xunit;

namespace School.UnitTests;

public class TenantEntityTests
{
    [Fact]
    public void Tenant_Create_NormalizesCodeToLowercaseAndSetsDefaults()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var tenant = Tenant.Create(id, "Fidel Hub Academy", "FIDELHUB", "Fidel Hub PLC", "info@fidelhub.com", "+251911000000");

        // Assert
        tenant.Id.Should().Be(id);
        tenant.Name.Should().Be("Fidel Hub Academy");
        tenant.Code.Should().Be("fidelhub");
        tenant.DefaultCurrency.Should().Be("ETB");
        tenant.Status.Should().Be("ACTIVE");
        tenant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Branch_Create_NormalizesCodeToUppercaseAndAssignsTenant()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var branch = Branch.Create(id, tenantId, "Main Bole Campus", "bole-main", true);

        // Assert
        branch.Id.Should().Be(id);
        branch.TenantId.Should().Be(tenantId);
        branch.Name.Should().Be("Main Bole Campus");
        branch.Code.Should().Be("BOLE-MAIN");
        branch.IsMainBranch.Should().BeTrue();
        branch.IsActive.Should().BeTrue();
    }
}
