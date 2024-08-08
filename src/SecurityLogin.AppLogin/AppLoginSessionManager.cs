using Ao.Cache;
using System.Threading.Tasks;
using System;

namespace SecurityLogin.AppLogin
{
    public abstract class AppLoginSessionManager<TAppInfoSnapshot>
        where TAppInfoSnapshot : IAppInfoSnapshot
    {
        public static readonly TimeSpan? DefaultCacheTime = TimeSpan.FromMinutes(10);
        private static readonly Task<TimeSpan?> defaultCacheTimeTask = Task.FromResult(DefaultCacheTime);

        private readonly ICacheVisitor cacheVisitor;

        protected abstract string GetKey(string appKey, TAppInfoSnapshot snapshot);
    }
}
