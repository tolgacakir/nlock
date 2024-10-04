namespace nlock.Abstraction;

public interface IDistributedLockFactory
{
    IDistributedLock CreateLock(string key, TimeSpan timeout, CancellationToken cancellationToken = default);
}
