using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin
{
    public abstract class SecurityLoginService<TFullKey>
        where TFullKey : IIdentityable
    {
        public static readonly TimeSpan DefaultKeyCacheTime = TimeSpan.FromMinutes(3);
        public static readonly TimeSpan DefaultLockerWaitTime = TimeSpan.FromSeconds(10);

        protected SecurityLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor)
        {
            LockerFactory = lockerFactory ?? throw new ArgumentNullException(nameof(lockerFactory));
            CacheVisitor = cacheVisitor ?? throw new ArgumentNullException(nameof(cacheVisitor));
        }

        public ILockerFactory LockerFactory { get; }
        public ICacheVisitor CacheVisitor { get; }

        protected abstract SecurityKeyIdentity Transfer(TFullKey fullKey);

        public async Task<SecurityKeyIdentity> FlushRSAKeyAsync()
        {
            var header = GetHeader();
            if (IsShared())
            {
                var identityKey = GetSharedIdentityKey();
                var identity = await CacheVisitor.GetStringAsync(identityKey);
                if (identity != null)
                {
                    var redisKey = KeyGenerator.Concat(header, identity);
                    var val = await CacheVisitor.GetAsync<TFullKey>(redisKey);
                    if (val != null)
                    {
                        return Transfer(val);
                    }
                }
                using (var locker = LockerFactory.CreateLock(GetSharedLockKey(), GetLockWaitTime()))
                {

                    if (locker.IsAcquired)
                    {
                        var rsaIdentity = GetFullKey();
                        await CacheVisitor.SetStringAsync(identityKey, rsaIdentity.Identity, GetCacheTime());
                        await SetRSAIdentityAsync(header, rsaIdentity);
                        return Transfer(rsaIdentity);
                    }
                }
                return null;
            }
            else
            {
                var rsaIdentity = GetFullKey();
                await SetRSAIdentityAsync(header, rsaIdentity);
                return Transfer(rsaIdentity);
            }
        }
        protected virtual bool IsShared()
        {
            return true;
        }
        protected virtual TimeSpan GetCacheTime()
        {
            return DefaultKeyCacheTime;
        }
        protected virtual TimeSpan GetLockWaitTime()
        {
            return DefaultLockerWaitTime;
        }
        private Task SetRSAIdentityAsync(string header, TFullKey fullKey)
        {
            var redisKey = KeyGenerator.Concat(header, fullKey.Identity);
            return CacheVisitor.SetAsync(redisKey, fullKey, GetCacheTime());
        }
        protected Task<TFullKey> GetFullKeyAsync(string header, string connectId)
        {
            var key = KeyGenerator.Concat(header, connectId);
            return CacheVisitor.GetAsync<TFullKey>(key);
        }
        protected Task DeleteKeyAsync(string connectId)
        {
            var header = GetHeader();
            var key = KeyGenerator.Concat(header, connectId);
            return CacheVisitor.DeleteAsync(key);
        }
        protected abstract TFullKey GetFullKey();
        protected abstract string GetHeader();
        protected abstract string GetSharedIdentityKey();
        protected abstract string GetSharedLockKey();

    }
}
