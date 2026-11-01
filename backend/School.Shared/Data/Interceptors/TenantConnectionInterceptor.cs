using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using School.Shared.Tenancy;

namespace School.Shared.Data.Interceptors;

/// <summary>
/// Intercepts database connections to inject tenant and branch context into PostgreSQL session settings.
/// Uses 'SET LOCAL' to guarantee multi-tenant safety when operating with PgBouncer in transaction pooling mode.
/// </summary>
public class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentTenantContext _tenantContext;

    public TenantConnectionInterceptor(ICurrentTenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetTenantSession(connection);
        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await SetTenantSessionAsync(connection, cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private void SetTenantSession(DbConnection connection)
    {
        if (connection is not NpgsqlConnection npgsqlConnection || !_tenantContext.IsResolved)
            return;

        if (npgsqlConnection.State != System.Data.ConnectionState.Open)
            return;

        using var cmd = npgsqlConnection.CreateCommand();
        cmd.CommandText = "SELECT set_config('app.current_tenant_id', @tenantId, false), set_config('app.current_branch_id', @branchId, false);";
        cmd.Parameters.Add(new NpgsqlParameter("tenantId", _tenantContext.TenantId?.ToString() ?? ""));
        cmd.Parameters.Add(new NpgsqlParameter("branchId", _tenantContext.BranchId?.ToString() ?? ""));
        cmd.ExecuteNonQuery();
    }

    private async Task SetTenantSessionAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        if (connection is not NpgsqlConnection npgsqlConnection || !_tenantContext.IsResolved)
            return;

        if (npgsqlConnection.State != System.Data.ConnectionState.Open)
            return;

        await using var cmd = npgsqlConnection.CreateCommand();
        cmd.CommandText = "SELECT set_config('app.current_tenant_id', @tenantId, false), set_config('app.current_branch_id', @branchId, false);";
        cmd.Parameters.Add(new NpgsqlParameter("tenantId", _tenantContext.TenantId?.ToString() ?? ""));
        cmd.Parameters.Add(new NpgsqlParameter("branchId", _tenantContext.BranchId?.ToString() ?? ""));
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
