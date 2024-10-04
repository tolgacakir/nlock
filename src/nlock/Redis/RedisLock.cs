using Microsoft.Extensions.Caching.Distributed;
using nlock.Abstraction;

namespace nlock.Redis;

public class RedisLock : IDistributedLock
{
    private readonly string _key;
    private readonly string _lockValue;
    private readonly TimeSpan _expiry;
    private readonly IDistributedCache _cacheClient;

    public RedisLock(IDistributedCache cacheClient, string key, string lockValue, TimeSpan expiry)
    {
        _cacheClient = cacheClient ?? throw new ArgumentNullException(nameof(cacheClient));
        _key = key ?? throw new ArgumentNullException(nameof(key));
        _lockValue = lockValue ?? throw new ArgumentNullException(nameof(lockValue));

        if (expiry <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(expiry), "Expiry must be greater than zero.");
        }

        _expiry = expiry;
    }

    public async Task<bool> AcquireAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        await _cacheClient.SetStringAsync(_key, _lockValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _expiry
        }, cancellationToken);

        return true;
    }

    public async ValueTask DisposeAsync()
    {
        await ReleaseAsync();
    }

    public async Task<bool> IsLockedAsync(CancellationToken cancellationToken = default)
    {
        return await _cacheClient.GetStringAsync(_key, cancellationToken) == _lockValue;
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken = default)
    {
        var value = await _cacheClient.GetStringAsync(_key, cancellationToken);
        if (value == _lockValue)
        {
            await _cacheClient.RemoveAsync(_key, cancellationToken);
        }
    }
}
