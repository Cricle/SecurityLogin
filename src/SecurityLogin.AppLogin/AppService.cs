using SecurityLogin.AppLogin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.AppLogin
{
    public abstract class AppService : AppService<IAppInfo, IAppInfoSnapshot, AppLoginResult, AppServiceOptions>
    {
        public AppService(IKeyGenerator keyGenerator, ITimeHelper timeHelper, ICacheVisitor cacheVisitor)
            : base(keyGenerator, timeHelper, cacheVisitor)
        {
        }

    }
    public abstract class AppService<TAppInfo,TAppInfoSnapshot,TAppLoginResult, TOptions>
        where TAppInfo : IAppInfo
        where TAppInfoSnapshot:IAppInfoSnapshot
        where TAppLoginResult:AppLoginResult,new()
        where TOptions : AppServiceOptions
    {
        public static readonly TimeSpan NotExistsCacheTime = TimeSpan.FromSeconds(3);
        public static readonly TimeSpan AppInfoCacheTime = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan DefaultTimestampTimeOut = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan DefaultSessionTime = TimeSpan.FromHours(6);

        public AppService(IKeyGenerator keyGenerator, ITimeHelper timeHelper, ICacheVisitor cacheVisitor)
        {
            KeyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
            TimeHelper = timeHelper ?? throw new ArgumentNullException(nameof(timeHelper));
            CacheVisitor = cacheVisitor ?? throw new ArgumentNullException(nameof(cacheVisitor));
        }

        public IKeyGenerator KeyGenerator { get; }

        public ITimeHelper TimeHelper { get; }

        public ICacheVisitor CacheVisitor { get; }

        protected virtual string GetNotExistKey(string appKey)
        {
            return "Red." + GetInfoKey(appKey);
        }
        protected virtual string GetSectionKey(string appKey,string session)
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

        private async Task<TAppInfoSnapshot> GetAppAsync(string appKey)
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

        protected virtual string GenerateToken(string appKey,long timestamp)
        {
            return Guid.NewGuid().ToString("N");
        }
        protected virtual TimeSpan? GetSessionCacheTime(string appKey,long timestamp,TimeSpan? minTime)
        {
            return minTime ?? DefaultSessionTime;
        }
        protected virtual bool IsTimestampTimeOut(string appKey,long timestamp,in DateTime now)
        {
            var csTime = TimeHelper.ToDateTime(timestamp);
            var subTime = Math.Abs((now - csTime).TotalMilliseconds);
            return subTime >= GetTimestampTimeOut(appKey,timestamp,now).TotalMilliseconds;
        }
        protected virtual TimeSpan GetTimestampTimeOut(string appKey, long timestamp, in DateTime now)
        {
            return DefaultTimestampTimeOut;
        }
        protected virtual Task WriteSessionAsync(string appKey, long timestamp,in TimeSpan? cacheTime, TAppLoginResult result)
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
            var mysignStr = appKey + timestamp + sn.AppSecret;
            var mysign = Md5Helper.ComputeHashToString(mysignStr);
            if (!mysign.Equals(sign, StringComparison.OrdinalIgnoreCase))
            {
                return new TAppLoginResult { Code = AppLoginCode.SignError };
            }
            var accessToken = GenerateToken(appKey,timestamp);
            var tokenTime = GetSessionCacheTime(appKey, timestamp, null);
            if (sn.EndTime != null)
            {
                var minTime = sn.EndTime.Value - now;
                minTime = tokenTime.HasValue ?
                    TimeSpan.FromMilliseconds(Math.Min(minTime.TotalMilliseconds, tokenTime.Value.TotalMilliseconds))
                    : minTime;
                tokenTime = GetSessionCacheTime(appKey, timestamp, minTime);
            }
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
        protected virtual Task FillAppLoginResultAsync(string appKey, long timestamp, in TimeSpan? cacheTime, TAppLoginResult result)
        {
            return Task.CompletedTask;
        }

    }
}
