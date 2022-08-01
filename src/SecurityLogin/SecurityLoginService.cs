using Ao.Cache;
using System;
using System.Threading.Tasks;

namespace SecurityLogin
{
    public abstract class SecurityLoginService<TFullKey> : ISecurityLoginService<TFullKey>
        where TFullKey : IIdentityable
    {
        public static readonly TimeSpan DefaultKeyCacheTime = TimeSpan.FromHours(1);
        public static readonly TimeSpan DefaultLockerWaitTime = TimeSpan.FromSeconds(10);

        protected SecurityLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor)
        {
            LockerFactory = lockerFactory ?? throw new ArgumentNullException(nameof(lockerFactory));
            CacheVisitor = cacheVisitor ?? throw new ArgumentNullException(nameof(cacheVisitor));
        }

        public ILockerFactory LockerFactory { get; }

        public ICacheVisitor CacheVisitor { get; }

        static async Task<TFullKey> GetAsync(ICacheVisitor cacheVisitor,string header,string identityKey)
        {
            var identity = await cacheVisitor.GetStringAsync(identityKey);
            if (identity != null)
            {
                var redisKey = KeyGenerator.Concat(header, identity);
                var val = await cacheVisitor.GetAsync<TFullKey>(redisKey);
                if (val != null)
                {
                    return val;
                }
            }
            return default;
        }
        public async Task<TFullKey> FlushKeyAsync()
        {
            var header = GetHeader();
            if (IsShared())
            {
                var identityKey = GetSharedIdentityKey();

                var fullKey= await GetAsync(CacheVisitor, header, identityKey);
                if (fullKey != null)
                {
                    return fullKey;
                }

                using (var locker = LockerFactory.CreateLock(GetSharedLockKey(), GetLockWaitTime()))
                {
                    if (locker.IsAcquired)
                    {
                        fullKey = await GetAsync(CacheVisitor, header, identityKey);
                        if (fullKey == null)
                        {
                            fullKey = GetFullKey();
                            await CacheVisitor.SetStringAsync(identityKey, fullKey.Identity, GetKeyCacheTime());
                            await SetRSAIdentityAsync(header, fullKey);
                            return fullKey;
                        }
                        return fullKey;
                    }
                }
                return default;
            }
            else
            {
                var fullKey = GetFullKey();
                await SetRSAIdentityAsync(header, fullKey);
                return fullKey;
            }
        }
        protected virtual bool IsShared()
        {
            return true;
        }
        protected virtual TimeSpan GetKeyCacheTime()
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
            return CacheVisitor.SetAsync(redisKey, fullKey, GetKeyCacheTime());
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
        protected virtual string GetHeader()
        {
            return TypeNameHelper.GetFriendlyFullName(GetType());
        }
        protected virtual string GetSharedIdentityKey()
        {
            return GetHeader() + ".Identity";
        }
        protected virtual string GetSharedLockKey()
        {
            return "Lock." + GetHeader() + ".Shared";
        }
        public async Task<string> DecryptAsync(string connectId, string textHash, IEncryptor<TFullKey> encrypter)
        {
            if (encrypter is null)
            {
                throw new ArgumentNullException(nameof(encrypter));
            }

            var header = GetHeader();
            var fullKey = await GetFullKeyAsync(header, connectId);
            if (fullKey == null)
            {
                return null;
            }
            return encrypter.DecryptToString(fullKey, textHash);
        }
    }
}
