using RedLockNet;
using System;

namespace SecurityLogin.Store.Redis
{
    internal class RedisLocker : ILocker
    {
        public RedisLocker(IRedLock locker)
        {
            Locker = locker;
        }

        public IRedLock Locker { get; }

        public string Resource => Locker.Resource;

        public string LockId => Locker.LockId;

        public bool IsAcquired => Locker.IsAcquired;

        public DateTime CreateTime { get;set; }

        public TimeSpan ExpireTime { get;set; }

        public void Dispose()
        {
            Locker.Dispose();
        }
    }
}
