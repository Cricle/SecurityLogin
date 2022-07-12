using System;
using System.Threading.Tasks;

namespace SecurityLogin.AccessSession
{
    public class IdentityService<TInput, TTokenInfo> : IIdentityService<TInput, TTokenInfo>
    {
        public static readonly TimeSpan? DefaultCacheTime = TimeSpan.FromMinutes(10);
        private static readonly Task<TimeSpan?> defaultCacheTimeTask = Task.FromResult(DefaultCacheTime);

        public IdentityService(ICacheVisitor cacheVisitor)
        {
            CacheVisitor = cacheVisitor ?? throw new ArgumentNullException(nameof(cacheVisitor));
        }

        public ICacheVisitor CacheVisitor { get; }

        public virtual Task<TTokenInfo> GetTokenInfoAsync(string token)
        {
            var key = GetKey(token);
            return CacheVisitor.GetAsync<TTokenInfo>(key);
        }
        public virtual Task<bool> ExistsAsync(string token)
        {
            var key = GetKey(token);
            return CacheVisitor.ExistsAsync(key);
        }

        public virtual Task<bool> DeleteAsync(string token)
        {
            var key = GetKey(token);
            return CacheVisitor.DeleteAsync(key);
        }

        public virtual async Task<bool> RenewAsync(string token)
        {
            var key = GetKey(token);
            var cacheTime = await GetCacheTimeAsync(token, default);
            var res = await CacheVisitor.ExpireAsync(key, cacheTime);
            return res;
        }

        public virtual async Task<string> IssureTokenAsync(TInput input)
        {
            var token = await GenerateTokenAsync(input);
            var key = GetKey(token);
            var cacheTime = await GetCacheTimeAsync(token, input);
            var ok=await CacheVisitor.SetAsync(key, input, cacheTime);
            if (!ok)
            {
                token = await FailSetCacheAsync(input, token, cacheTime);
            }
            return token;
        }

        protected virtual Task<string> FailSetCacheAsync(TInput input,string token,TimeSpan? cacheTime)
        {
            throw new InvalidOperationException($"Fail to set cache with {token}");
        }

        protected virtual Task<TimeSpan?> GetCacheTimeAsync(string token, TInput input)
        {
            return defaultCacheTimeTask;
        }
        protected virtual Task<string> GenerateTokenAsync(TInput input)
        {
            return Task.FromResult(Guid.NewGuid().ToString("N"));
        }

        protected virtual string GetKey(string token)
        {
            var name = TypeNameHelper.GetFriendlyFullName(GetType());
            return KeyGenerator.Concat(name, token);
        }
    }
}
