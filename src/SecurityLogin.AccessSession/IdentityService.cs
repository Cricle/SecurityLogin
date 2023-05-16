using Ao.Cache;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AccessSession
{
    public readonly record struct IdentityAsTokenInfoRequest<TInput>
    {
        public readonly TInput Input;

        public readonly TimeSpan? CacheTime;

        public readonly string Key;

        public readonly string Token;

        public IdentityAsTokenInfoRequest(TInput input, TimeSpan? cacheTime, string key, string token)
        {
            Input = input;
            CacheTime = cacheTime;
            Key = key;
            Token = token;
        }
    }
    public delegate Task<TTokenInfo> IdentityAsTokenInfoHandler<TInput, TTokenInfo>(IdentityAsTokenInfoRequest<TInput> request);
    public delegate string IdentityGetKeyHandler(string token);
    public delegate Task<string> IdentityGenerateTokenHandler<TInput>(TInput input);

    public class DelegateIdentityService<TInput, TTokenInfo> : IdentityService<TInput, TTokenInfo>
    {
        public DelegateIdentityService(ICacheVisitor cacheVisitor,
            IdentityAsTokenInfoHandler<TInput, TTokenInfo> asTokenInfoHandler,
            IdentityGetKeyHandler? getKeyHandler=null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler=null)
            : base(cacheVisitor)
        {
            AsTokenInfoHandler = asTokenInfoHandler;
            GetKeyHandler = getKeyHandler;
            GenerateTokenHandler = generateTokenHandler;
        }

        public IdentityAsTokenInfoHandler<TInput, TTokenInfo> AsTokenInfoHandler { get; }

        public IdentityGetKeyHandler? GetKeyHandler { get; }

        public IdentityGenerateTokenHandler<TInput>? GenerateTokenHandler { get; }

        protected override Task<TTokenInfo> AsTokenInfoAsync(TInput input, TimeSpan? cacheTime, string key, string token)
        {
            return AsTokenInfoHandler(new IdentityAsTokenInfoRequest<TInput>(input,cacheTime,key,token));
        }
        protected override Task<string> GenerateTokenAsync(TInput input)
        {
            if (GenerateTokenHandler != null)
            {
                return GenerateTokenHandler(input);
            }
            return base.GenerateTokenAsync(input);
        }
        protected override string GetKey(string token)
        {
            if (GetKeyHandler!=null)
            {
                return GetKeyHandler(token);
            }
            return base.GetKey(token);
        }
    }
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
