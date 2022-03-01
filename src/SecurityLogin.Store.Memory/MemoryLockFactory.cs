using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Store.Memory
{
    public class MemoryLockFactory : ILockerFactory
    {
        private readonly object syncRoot = new object();
        private readonly Dictionary<string, MemoryLocker> resourceMap = new Dictionary<string, MemoryLocker>();

        public ILocker CreateLock(string resource, TimeSpan expiryTime)
        {
            lock (syncRoot)
            {
                var now = DateTime.Now;
                if (resourceMap.TryGetValue(resource,out var locker)&& !locker.IsInvalid)
                {
                    return new MemoryLocker
                    {
                        CreateTime = now,
                        ExpireTime = expiryTime,
                        Resource = resource,
                        IsAcquired = false,
                        LockId = Guid.NewGuid().ToString(),
                        MemoryLockFactory = this
                    };
                }
                locker= new MemoryLocker
                {
                    CreateTime = now,
                    ExpireTime = expiryTime,
                    Resource = resource,
                    IsAcquired = true,
                    LockId = Guid.NewGuid().ToString(),
                    MemoryLockFactory = this
                };
                resourceMap[resource] = locker;
                return locker;
            }
        }

        public Task<ILocker> CreateLockAsync(string resource, TimeSpan expiryTime)
        {
            return Task.FromResult(CreateLock(resource, expiryTime));
        }
        internal void Remove(string resource)
        {
            resourceMap.Remove(resource);
        }
    }
}
