namespace nlock.Abstraction;

public interface IDistributedLock : IAsyncDisposable
{
    Task<bool> AcquireAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    Task ReleaseAsync(CancellationToken cancellationToken = default);
    Task<bool> IsLockedAsync(CancellationToken cancellationToken = default);
}
