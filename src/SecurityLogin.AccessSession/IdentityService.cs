using Ao.Cache;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AccessSession
{
    public delegate Task<TTokenInfo> IdentityAsTokenInfoHandler<TInput, TTokenInfo>(IdentityAsTokenInfoRequest<TInput> request);
    public delegate string IdentityGetKeyHandler(string token);
    public delegate Task<string> IdentityGenerateTokenHandler<TInput>(TInput input);
    public class IdentityService<T> : IdentityService<T, T>
    {
        public IdentityService(ICacheVisitor cacheVisitor) : base(cacheVisitor)
        {
        }

        protected override Task<T> AsTokenInfoAsync(T input, TimeSpan? cacheTime, string key, string token)
        {
            return Task.FromResult(input);
        }
    }
    public abstract class IdentityService<TInput, TTokenInfo> : IIdentityService<TInput, TTokenInfo>
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
        public virtual async Task<bool> UpdateAsync(string token,TTokenInfo info,CacheSetIf cacheSetIf= CacheSetIf.Always)
        {
            var key = GetKey(token);
            var cacheTime = await GetCacheTimeAsync(token, default);
            var res = await CacheVisitor.SetAsync(key, info, cacheTime, cacheSetIf);
            return res;
        }
        protected abstract Task<TTokenInfo> AsTokenInfoAsync(TInput input, TimeSpan? cacheTime, string key, string token);

        public virtual async Task<IssureTokenResult> IssureTokenAsync(TInput input)
        {
            var token = await GenerateTokenAsync(input);
            var key = GetKey(token);
            var cacheTime = await GetCacheTimeAsync(token, input);
            var output = await AsTokenInfoAsync(input, cacheTime, key, token);
            var ok = await CacheVisitor.SetAsync(key, output, cacheTime);
            if (!ok)
            {
                token = await FailSetCacheAsync(input, token, cacheTime);
            }
            var res = new IssureTokenResult(
                token, cacheTime, ok);
            return res;
        }

        protected virtual Task<string> FailSetCacheAsync(TInput input, string token, TimeSpan? cacheTime)
        {
            throw new InvalidOperationException($"Fail to set cache with {token}");
        }

        protected virtual Task<TimeSpan?> GetCacheTimeAsync(string token, TInput? input)
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
