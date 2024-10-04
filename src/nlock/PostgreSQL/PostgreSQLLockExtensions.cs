using Microsoft.Extensions.DependencyInjection;
using nlock.Abstraction;
using Npgsql;

namespace nlock.PostgreSQL;

public static class PostgreSQLLockExtensions
{
    public static IServiceCollection AddPostgreSQLDistributedLock(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<NpgsqlConnection>(sp =>
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        });

        services.AddSingleton<IDistributedLockFactory, PostgreSQLLockFactory>();

        return services;
    }
}
