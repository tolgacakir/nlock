using nlock.Abstraction;
using Npgsql;
using System.Diagnostics;

namespace nlock.PostgreSQL;

public class PostgreSQLLock : IDistributedLock
{
    private readonly NpgsqlConnection _connection;
    private readonly long _lockKey;

    public PostgreSQLLock(NpgsqlConnection connection, long lockKey)
    {
        _connection = connection;
        _lockKey = lockKey;
    }

    public async Task<bool> AcquireAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var sw = new Stopwatch();
        sw.Start();

        try
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                await _connection.OpenAsync(cancellationToken);
            }

            while (sw.Elapsed < timeout)
            {
                using var command = new NpgsqlCommand("SELECT pg_try_advisory_lock(@lockKey)", _connection);
                command.Parameters.AddWithValue("lockKey", _lockKey);
                var result = await command.ExecuteScalarAsync(cancellationToken);

                if (result is true)
                {
                    return true;
                }

                await Task.Delay(100, cancellationToken);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return false;
    }

    public async Task<bool> IsLockedAsync(CancellationToken cancellationToken = default)
    {
        if (_connection.State == System.Data.ConnectionState.Closed)
        {
            await _connection.OpenAsync(cancellationToken);
        }

        using var command = new NpgsqlCommand("SELECT pg_advisory_lock(@lockKey)", _connection);
        command.Parameters.AddWithValue("lockKey", _lockKey);
        var result = await command.ExecuteScalarAsync();
        return result is true;
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken = default)
    {
        if (_connection.State == System.Data.ConnectionState.Closed)
        {
            await _connection.OpenAsync(cancellationToken);
        }

        using var command = new NpgsqlCommand("SELECT pg_advisory_unlock(@lockKey)", _connection);
        command.Parameters.AddWithValue("lockKey", _lockKey);
        await command.ExecuteNonQueryAsync();
    }

    public async Task CloseConnectionAsync()
    {
        if (_connection.State == System.Data.ConnectionState.Open)
        {
            await _connection.CloseAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await ReleaseAsync();
        await CloseConnectionAsync();
        _connection.Dispose();
    }
}