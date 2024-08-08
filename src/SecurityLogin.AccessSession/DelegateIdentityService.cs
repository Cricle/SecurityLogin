using Ao.Cache;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AccessSession
{
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
}
