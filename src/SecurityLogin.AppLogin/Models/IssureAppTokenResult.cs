using System;

namespace SecurityLogin.AppLogin.Models
{
    public class IssureAppTokenResult
    {
        public IssureAppTokenResult(string token, TimeSpan? expireTime, bool setSucceed)
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
