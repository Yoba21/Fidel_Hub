using System.Data;
using System.Data.Common;
using FluentAssertions;
using Moq;
using School.Shared.Data.Interceptors;
using School.Shared.Tenancy;
using Xunit;

namespace School.UnitTests;

public class TenantConnectionInterceptorTests
{
    private class TestDbTransaction : DbTransaction
    {
        private readonly DbConnection? _connection;
        public TestDbTransaction(DbConnection? connection = null) => _connection = connection;
        protected override DbConnection? DbConnection => _connection;
        public override IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
        public override void Commit() { }
        public override void Rollback() { }
    }

    [Fact]
    public void TransactionStarted_WhenConnectionIsNull_SafelyNoOps()
    {
        // Arrange
        var tenantContext = new CurrentTenantContext();
        var interceptor = new TenantConnectionInterceptor(tenantContext);
        var tx = new TestDbTransaction(null);

        // Act
        var act = () => interceptor.TransactionStarted(tx, default!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task TransactionStartedAsync_WhenConnectionIsNull_SafelyNoOps()
    {
        // Arrange
        var tenantContext = new CurrentTenantContext();
        var interceptor = new TenantConnectionInterceptor(tenantContext);
        var tx = new TestDbTransaction(null);

        // Act
        var act = async () => await interceptor.TransactionStartedAsync(tx, default!);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void TransactionStarted_WhenConnectionNotNpgsql_SafelyReturnsWithoutException()
    {
        // Arrange
        var tenantContext = new CurrentTenantContext();
        tenantContext.SetContext(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new[] { "ADMIN" });
        var interceptor = new TenantConnectionInterceptor(tenantContext);

        var mockConn = new Mock<DbConnection>();
        mockConn.Setup(c => c.State).Returns(ConnectionState.Open);
        var tx = new TestDbTransaction(mockConn.Object);

        // Act
        var act = () => interceptor.TransactionStarted(tx, default!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task TransactionStartedAsync_WhenConnectionNotNpgsql_SafelyReturnsWithoutException()
    {
        // Arrange
        var tenantContext = new CurrentTenantContext();
        tenantContext.SetContext(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new[] { "ADMIN" });
        var interceptor = new TenantConnectionInterceptor(tenantContext);

        var mockConn = new Mock<DbConnection>();
        mockConn.Setup(c => c.State).Returns(ConnectionState.Open);
        var tx = new TestDbTransaction(mockConn.Object);

        // Act
        var act = async () => await interceptor.TransactionStartedAsync(tx, default!);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void ConnectionClosing_WhenNotNpgsql_SafelyReturnsWithoutException()
    {
        // Arrange
        var tenantContext = new CurrentTenantContext();
        var interceptor = new TenantConnectionInterceptor(tenantContext);
        var mockConn = new Mock<DbConnection>();

        // Act
        var act = () => interceptor.ConnectionClosing(mockConn.Object, default!, default);

        // Assert
        act.Should().NotThrow();
    }
}
