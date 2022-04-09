using SecurityLogin.AppLogin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.AppLogin
{
    public class AppLoginResult
    {
        public AppLoginCode Code { get; set; }

        public string AccessToken { get; set; }

        public TimeSpan? ExpireTime { get; set; }

        public DateTime CreateAt { get; set; }
    }
    public abstract class AppService<TAppInfo,TAppInfoSnapshot, TOptions>
        where TAppInfo : IAppInfo
        where TOptions : AppServiceOptions
        where TAppInfoSnapshot:IAppInfoSnapshot
    {
        public static readonly TimeSpan NotExistsCacheTime = TimeSpan.FromSeconds(3);
        public static readonly TimeSpan AppInfoCacheTime = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan DefaultTimestampTimeOut = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan DefaultSessionTime = TimeSpan.FromHours(6);

        public IKeyGenerator KeyGenerator { get; }

        public ITimeHelper TimeHelper { get; }

        public ICacheVisitor CacheVisitor { get; }

        protected abstract string GetNotExistKey(string appKey);
        protected abstract string GetInfoKey(string appKey);

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
            return subTime <= DefaultTimestampTimeOut.TotalMilliseconds;
        }
        protected abstract Task WriteSessionAsync(string appKey, long timestamp,in TimeSpan? cacheTime,AppLoginResult result);
        public abstract Task<bool> HasSessionAsync(string appKey, string session);
        public abstract AppLoginResult GetSessionAsync(string appKey, string session);

        public async Task<AppLoginResult> LoginAsync(string appKey, long timestamp, string sign)
        {
            var sn = await GetAppAsync(appKey);
            if (sn == null)
            {
                return new AppLoginResult { Code = AppLoginCode.NoSuchAppKey };
            }
            var now = DateTime.Now;
            if (sn.EndTime != null && sn.EndTime <= now)
            {
                return new AppLoginResult { Code = AppLoginCode.AppEndOfTime };
            }
            if (IsTimestampTimeOut(appKey, timestamp, now))
            {
                return new AppLoginResult { Code = AppLoginCode.TimestampOutOfTime };
            }
            var mysignStr = appKey + timestamp + sn.AppSecret;
            var mysign = Md5Helper.ComputeHashToString(mysignStr);
            if (!mysign.Equals(sign, StringComparison.OrdinalIgnoreCase))
            {
                return new AppLoginResult { Code = AppLoginCode.SignError };
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
            var session = new AppLoginResult
            {
                CreateAt = now,
                AccessToken = accessToken,
                ExpireTime = tokenTime
            };
            await WriteSessionAsync(appKey,timestamp, tokenTime, session);
            return session;
        }

    }
}
