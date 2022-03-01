using System;
using System.Threading.Tasks;

namespace SecurityLogin
{
    public interface ILockerFactory
    {
        ILocker CreateLock(string resource, TimeSpan expiryTime);

        Task<ILocker> CreateLockAsync(string resource, TimeSpan expiryTime);
    }
}
