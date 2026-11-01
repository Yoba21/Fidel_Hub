using FluentAssertions;
using School.Shared.Tenancy;
using Xunit;

namespace School.UnitTests;

public class CurrentTenantContextTests
{
    [Fact]
    public void SetContext_WhenCalled_PopulatesAllPropertiesCorrectly()
    {
        // Arrange
        var context = new CurrentTenantContext();
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roles = new[] { "TEACHER", "EXAM_COORDINATOR" };
        var permissions = new[] { "att.session.take", "grade.marksheet.enter" };

        // Act
        context.SetContext(tenantId, branchId, userId, roles, permissions, "fidelhub");

        // Assert
        context.IsResolved.Should().BeTrue();
        context.TenantId.Should().Be(tenantId);
        context.BranchId.Should().Be(branchId);
        context.UserId.Should().Be(userId);
        context.TenantCode.Should().Be("fidelhub");
        context.Roles.Should().BeEquivalentTo(roles);
        context.Permissions.Should().BeEquivalentTo(permissions);
    }

    [Fact]
    public void HasPermission_WhenUserPossessesPermission_ReturnsTrue()
    {
        // Arrange
        var context = new CurrentTenantContext();
        context.SetContext(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new[] { "TEACHER" }, new[] { "att.session.take" });

        // Act & Assert
        context.HasPermission("att.session.take").Should().BeTrue();
        context.HasPermission("fee.payment.record").Should().BeFalse();
    }

    [Fact]
    public void HasPermission_WhenUserIsSuperAdmin_ReturnsTrueForAnyPermission()
    {
        // Arrange
        var context = new CurrentTenantContext();
        context.SetContext(Guid.NewGuid(), null, Guid.NewGuid(), new[] { "PLATFORM_SUPERADMIN" }, Array.Empty<string>());

        // Act & Assert
        context.HasPermission("any.unlisted.permission").Should().BeTrue();
    }

    [Fact]
    public void IsInRole_WhenCaseDiffers_ReturnsTrueCaseInsensitive()
    {
        // Arrange
        var context = new CurrentTenantContext();
        context.SetContext(Guid.NewGuid(), null, Guid.NewGuid(), new[] { "TEACHER" });

        // Act & Assert
        context.IsInRole("teacher").Should().BeTrue();
        context.IsInRole("TEACHER").Should().BeTrue();
        context.IsInRole("PRINCIPAL").Should().BeFalse();
    }
}
