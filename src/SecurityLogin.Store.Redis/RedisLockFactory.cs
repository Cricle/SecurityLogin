using RedLockNet;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.Store.Redis
{
    public class RedisLockFactory : ILockerFactory
    {
        public RedisLockFactory(IDistributedLockFactory lockFactory)
        {
            LockFactory = lockFactory ?? throw new ArgumentNullException(nameof(lockFactory));
        }

        public IDistributedLockFactory LockFactory { get; }

        public ILocker CreateLock(string resource, TimeSpan expiryTime)
        {
             var locker=LockFactory.CreateLock(resource, expiryTime);
            return new RedisLocker(locker) { CreateTime = DateTime.Now, ExpireTime = expiryTime };
        }

        public async Task<ILocker> CreateLockAsync(string resource, TimeSpan expiryTime)
        {
            var locker =await LockFactory.CreateLockAsync(resource, expiryTime);
            return new RedisLocker(locker) { CreateTime = DateTime.Now, ExpireTime = expiryTime };
        }
    }
}
