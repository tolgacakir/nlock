using nlock.Abstraction;
using Npgsql;

namespace nlock.PostgreSQL;

public class PostgreSQLLockFactory : IDistributedLockFactory
{
    private readonly NpgsqlConnection _connection;

    public PostgreSQLLockFactory(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public IDistributedLock CreateLock(string key, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        long lockKey = GenerateLockKey(key);
        return new PostgreSQLLock(_connection, lockKey);
    }

    private long GenerateLockKey(string key)
    {
        return BitConverter.ToInt64(System.Text.Encoding.UTF8.GetBytes(key.PadRight(8).Substring(0, 8)), 0);
    }
}
