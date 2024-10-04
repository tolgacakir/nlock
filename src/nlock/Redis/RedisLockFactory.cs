using Microsoft.Extensions.Caching.Distributed;
using nlock.Abstraction;

namespace nlock.Redis;

public class RedisLockFactory : IDistributedLockFactory
{
    private readonly IDistributedCache _cacheClient;

    public RedisLockFactory(IDistributedCache cacheClient)
    {
        _cacheClient = cacheClient;
    }

    public IDistributedLock CreateLock(string key, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var lockValue = Guid.NewGuid().ToString();
        var expiry = timeout;

        return new RedisLock(_cacheClient, key, lockValue, expiry);
    }
}
