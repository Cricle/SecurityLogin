using System;

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
}
