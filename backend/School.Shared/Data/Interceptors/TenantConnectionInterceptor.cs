using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;
using School.Shared.Tenancy;

namespace School.Shared.Data.Interceptors;

/// <summary>
/// Intercepts database connections and transactions to inject tenant and branch context into PostgreSQL session settings.
/// Guarantees multi-tenant safety by setting session configs on checkout, resetting on close,
/// and setting transaction-local configs (is_local = true) on transaction start.
/// Fails closed if session scoping cannot be established.
/// </summary>
public class TenantConnectionInterceptor : DbConnectionInterceptor, IDbTransactionInterceptor
{
    private readonly ICurrentTenantContext _tenantContext;
    private readonly ILogger<TenantConnectionInterceptor>? _logger;

    public TenantConnectionInterceptor(ICurrentTenantContext tenantContext, ILogger<TenantConnectionInterceptor>? logger = null)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetTenantSession(connection, isLocal: false);
        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await SetTenantSessionAsync(connection, isLocal: false, cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override InterceptionResult ConnectionClosing(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        ResetTenantSession(connection);
        return base.ConnectionClosing(connection, eventData, result);
    }

    public override async ValueTask<InterceptionResult> ConnectionClosingAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        await ResetTenantSessionAsync(connection);
        return await base.ConnectionClosingAsync(connection, eventData, result);
    }

    public void TransactionStarted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        if (transaction.Connection != null)
        {
            SetTenantSession(transaction.Connection, isLocal: true, transaction as NpgsqlTransaction);
        }
    }

    public async Task TransactionStartedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        if (transaction.Connection != null)
        {
            await SetTenantSessionAsync(transaction.Connection, isLocal: true, cancellationToken, transaction as NpgsqlTransaction);
        }
    }

    public InterceptionResult TransactionStarting(DbConnection connection, TransactionStartingEventData eventData, InterceptionResult result) => result;
    public ValueTask<InterceptionResult> TransactionStartingAsync(DbConnection connection, TransactionStartingEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default) => ValueTask.FromResult(result);
    public DbTransaction TransactionUsed(DbConnection connection, TransactionEventData eventData, DbTransaction result) => result;
    public ValueTask<DbTransaction> TransactionUsedAsync(DbConnection connection, TransactionEventData eventData, DbTransaction result, CancellationToken cancellationToken = default) => ValueTask.FromResult(result);
    public void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData) { }
    public Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData) { }
    public Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData) { }
    public Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default) => Task.CompletedTask;

    private void SetTenantSession(DbConnection connection, bool isLocal, NpgsqlTransaction? transaction = null)
    {
        if (connection is not NpgsqlConnection npgsqlConnection || !_tenantContext.IsResolved)
            return;

        if (npgsqlConnection.State != System.Data.ConnectionState.Open)
            return;

        try
        {
            using var cmd = npgsqlConnection.CreateCommand();
            if (transaction != null) cmd.Transaction = transaction;

            cmd.CommandText = isLocal
                ? "SELECT set_config('app.current_tenant_id', @tenantId, true), set_config('app.current_branch_id', @branchId, true);"
                : "SELECT set_config('app.current_tenant_id', @tenantId, false), set_config('app.current_branch_id', @branchId, false);";
            
            cmd.Parameters.Add(new NpgsqlParameter("tenantId", _tenantContext.TenantId?.ToString() ?? ""));
            cmd.Parameters.Add(new NpgsqlParameter("branchId", _tenantContext.BranchId?.ToString() ?? ""));
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "FATAL: Failed to establish tenant session in database connection for tenant {TenantId}. Failing closed.", _tenantContext.TenantId);
            throw new InvalidOperationException($"Failed to establish tenant database session context for tenant '{_tenantContext.TenantId}'. Connection denied to prevent cross-tenant data exposure.", ex);
        }
    }

    private async Task SetTenantSessionAsync(DbConnection connection, bool isLocal, CancellationToken cancellationToken = default, NpgsqlTransaction? transaction = null)
    {
        if (connection is not NpgsqlConnection npgsqlConnection || !_tenantContext.IsResolved)
            return;

        if (npgsqlConnection.State != System.Data.ConnectionState.Open)
            return;

        try
        {
            await using var cmd = npgsqlConnection.CreateCommand();
            if (transaction != null) cmd.Transaction = transaction;

            cmd.CommandText = isLocal
                ? "SELECT set_config('app.current_tenant_id', @tenantId, true), set_config('app.current_branch_id', @branchId, true);"
                : "SELECT set_config('app.current_tenant_id', @tenantId, false), set_config('app.current_branch_id', @branchId, false);";

            cmd.Parameters.Add(new NpgsqlParameter("tenantId", _tenantContext.TenantId?.ToString() ?? ""));
            cmd.Parameters.Add(new NpgsqlParameter("branchId", _tenantContext.BranchId?.ToString() ?? ""));
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "FATAL: Failed to establish tenant session in database connection for tenant {TenantId}. Failing closed.", _tenantContext.TenantId);
            throw new InvalidOperationException($"Failed to establish tenant database session context for tenant '{_tenantContext.TenantId}'. Connection denied to prevent cross-tenant data exposure.", ex);
        }
    }

    private void ResetTenantSession(DbConnection connection)
    {
        if (connection is not NpgsqlConnection npgsqlConnection || npgsqlConnection.State != System.Data.ConnectionState.Open)
            return;

        try
        {
            using var cmd = npgsqlConnection.CreateCommand();
            cmd.CommandText = "SELECT set_config('app.current_tenant_id', '', false), set_config('app.current_branch_id', '', false);";
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to cleanly reset tenant session configuration on connection close.");
        }
    }

    private async Task ResetTenantSessionAsync(DbConnection connection)
    {
        if (connection is not NpgsqlConnection npgsqlConnection || npgsqlConnection.State != System.Data.ConnectionState.Open)
            return;

        try
        {
            await using var cmd = npgsqlConnection.CreateCommand();
            cmd.CommandText = "SELECT set_config('app.current_tenant_id', '', false), set_config('app.current_branch_id', '', false);";
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to cleanly reset tenant session configuration on connection close.");
        }
    }
}
