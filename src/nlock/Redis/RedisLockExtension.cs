using Microsoft.Extensions.DependencyInjection;
using nlock.Abstraction;

namespace nlock.Redis;

public static class RedisLockExtension
{
    public static IServiceCollection AddRedisDistributedLock(this IServiceCollection services, string connectionString, string prefix = "nlock")
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = prefix;
        });

        services.AddScoped<IDistributedLockFactory, RedisLockFactory>();
        return services;
    }

    public static IServiceCollection AddRedisDistributedLock(this IServiceCollection services)
    {
        services.AddScoped<IDistributedLockFactory, RedisLockFactory>();
        return services;
    }
}
