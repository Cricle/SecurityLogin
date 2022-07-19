using System;

namespace SecurityLogin.AccessSession
{
    public class IssureTokenResult
    {
        public IssureTokenResult(string token, TimeSpan? expireTime, bool setSucceed)
        {
            Token = token;
            ExpireTime = expireTime;
            SetSucceed = setSucceed;
        }

        public string Token { get; }

        public TimeSpan? ExpireTime { get; }

        public bool SetSucceed { get; }
    }
}
