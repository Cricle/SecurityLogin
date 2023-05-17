using Ao.Cache;
using SecurityLogin.AppLogin.Models;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AppLogin
{
    public abstract class AppService : AppService<IAppInfo, IAppInfoSnapshot, AppLoginResult>
    {
        protected AppService(IAppServiceProvider provider)
            : base(provider)
        {
        }
    }
    public abstract class AppService<TAppInfo, TAppInfoSnapshot, TAppLoginResult>
        where TAppInfo : IAppInfo
        where TAppInfoSnapshot : IAppInfoSnapshot
        where TAppLoginResult : AppLoginResult, new()
    {
        public static readonly TimeSpan NotExistsCacheTime = TimeSpan.FromSeconds(3);
        public static readonly TimeSpan AppInfoCacheTime = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan DefaultTimestampTimeOut = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan DefaultSessionTime = TimeSpan.FromHours(6);

        public AppService(IAppServiceProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IAppServiceProvider Provider { get; }

        public IKeyGenerator KeyGenerator => Provider.KeyGenerator;

        public ITimeHelper TimeHelper => Provider.TimeHelper;

        public ICacheVisitor CacheVisitor => Provider.CacheVisitor;

        public IEncryptionHelper EncryptionHelper => Provider.EncryptionHelper;

        protected virtual string GetNotExistKey(string appKey)
        {
            return "Red." + GetInfoKey(appKey);
        }
        protected virtual string GetSectionKey(string appKey, string? session)
        {
            return GetInfoKey(appKey) + "." + session;
        }
        protected virtual string GetInfoKey(string appKey)
        {
            return TypeNameHelper.GetFriendlyFullName(GetType()) + "." + appKey;
        }

        protected abstract Task<TAppInfoSnapshot> GetAppInfoSnapshotAsync(string appKey);

        protected virtual TimeSpan? GetNotExistsCacheTime(string appKey)
        {
            return NotExistsCacheTime;
        }
        protected virtual TimeSpan? GetInfoExistsCacheTime(string appKey)
        {
            return AppInfoCacheTime;
        }

        private async Task<TAppInfoSnapshot?> GetAppAsync(string appKey)
        {
            var notExistsKey = GetNotExistKey(appKey);
            var notExists = await CacheVisitor.ExistsAsync(notExistsKey);
            if (notExists)
            {
                return default;
            }
            var redisKey = GetInfoKey(appKey);
            var val = await CacheVisitor.GetAsync<TAppInfoSnapshot>(redisKey);
            if (val != null)
            {
                return val;
            }

            var data = await GetAppInfoSnapshotAsync(appKey);
            if (data == null)
            {
                await CacheVisitor.SetAsync(notExistsKey, false, GetNotExistsCacheTime(appKey));
                return default;
            }
            else
            {
                await CacheVisitor.SetAsync(redisKey, data, GetInfoExistsCacheTime(appKey));
                return data;
            }
        }

        protected virtual string GenerateToken(string appKey, long timestamp)
        {
            return Guid.NewGuid().ToString("N");
        }
        protected virtual TimeSpan? GetSessionCacheTime(string appKey, long timestamp, TimeSpan? minTime)
        {
            return minTime ?? DefaultSessionTime;
        }
        protected virtual bool IsTimestampTimeOut(string appKey, long timestamp, in DateTime now)
        {
            var csTime = TimeHelper.ToDateTime(timestamp);
            var subTime = Math.Abs((now - csTime).TotalMilliseconds);
            return subTime >= GetTimestampTimeOut(appKey, timestamp, now).TotalMilliseconds;
        }
        protected virtual TimeSpan GetTimestampTimeOut(string appKey, long timestamp, in DateTime now)
        {
            return DefaultTimestampTimeOut;
        }
        protected virtual Task WriteSessionAsync(string appKey, long timestamp, in TimeSpan? cacheTime, TAppLoginResult result)
        {
            var key = GetSectionKey(appKey, result.AccessToken);
            return CacheVisitor.SetAsync(key, result, cacheTime);
        }
        public virtual Task<bool> HasSessionAsync(string appKey, string session)
        {
            var key = GetSectionKey(appKey, session);
            return CacheVisitor.ExistsAsync(key);
        }
        public virtual Task<TAppLoginResult> GetSessionAsync(string appKey, string session)
        {
            var key = GetSectionKey(appKey, session);
            return CacheVisitor.GetAsync<TAppLoginResult>(key);
        }
        public virtual Task<bool> ExpireSessionAsync(string appKey, string session)
        {
            var key = GetSectionKey(appKey, session);
            var cacheTime = GetSessionCacheTime(appKey, 0, null);
            return CacheVisitor.ExpireAsync(key, cacheTime);
        }
        public virtual Task<bool> DeleteSessionAsync(string appKey, string session)
        {
            var key = GetSectionKey(appKey, session);
            return CacheVisitor.DeleteAsync(key);
        }

        public async Task<TAppLoginResult> LoginAsync(string appKey, long timestamp, string sign)
        {
            var sn = await GetAppAsync(appKey);
            if (sn == null)
            {
                return new TAppLoginResult { Code = AppLoginCode.NoSuchAppKey };
            }
            var now = DateTime.Now;
            if (sn.EndTime != null && sn.EndTime <= now)
            {
                return new TAppLoginResult { Code = AppLoginCode.AppEndOfTime };
            }
            if (IsTimestampTimeOut(appKey, timestamp, now))
            {
                return new TAppLoginResult { Code = AppLoginCode.TimestampOutOfTime };
            }
            var mysignStr = GetSignString(appKey, timestamp, sn);
            var mysign = EncryptionHelper.ComputeHashToString(mysignStr);
            if (!mysign.Equals(sign, StringComparison.OrdinalIgnoreCase))
            {
                return new TAppLoginResult { Code = AppLoginCode.SignError };
            }
            var accessToken = GenerateToken(appKey, timestamp);
            var tokenTime = GetSessionCacheTime(appKey, timestamp, null);
            tokenTime = GetTokenTime(appKey, timestamp, tokenTime, now, sn);
            var session = new TAppLoginResult
            {
                CreateAt = now,
                AccessToken = accessToken,
                ExpireTime = tokenTime
            };
            await FillAppLoginResultAsync(appKey, timestamp, tokenTime, session);
            await WriteSessionAsync(appKey, timestamp, tokenTime, session);
            return session;
        }

        protected virtual TimeSpan? GetTokenTime(string appKey, long timestamp, TimeSpan? givenTime, DateTime now, TAppInfoSnapshot snapshot)
        {
            if (snapshot.EndTime != null)
            {
                var minTime = snapshot.EndTime.Value - now;
                minTime = givenTime.HasValue ?
                    TimeSpan.FromMilliseconds(Math.Min(minTime.TotalMilliseconds, givenTime.Value.TotalMilliseconds))
                    : minTime;
                return GetSessionCacheTime(appKey, timestamp, minTime);
            }
            return givenTime;
        }

        protected virtual string GetSignString(string appKey, long timestamp, TAppInfoSnapshot snapshot)
        {
            return appKey + timestamp + snapshot.AppSecret;
        }
        protected virtual Task FillAppLoginResultAsync(string appKey, long timestamp, in TimeSpan? cacheTime, TAppLoginResult result)
        {
            return Task.CompletedTask;
        }

    }
}
