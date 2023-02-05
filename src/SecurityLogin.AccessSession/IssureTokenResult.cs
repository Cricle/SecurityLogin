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

    public class UserStatusContainer<TUserSnapshot>
        where TUserSnapshot : UserSnapshot
    {
        public AppSnapshot AppSnapshot { get; set; }

        public TUserSnapshot UserSnapshot { get; set; }

        public bool HasAppSnapshot => AppSnapshot != null;

        public bool HasUserSnapshot => UserSnapshot != null;
    }
    public class AppSnapshot
    {
        public string AppKey { get; set; }

        public string AppSession { get; set; }
    }
    public class UserSnapshot
    {
        public string Token { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
