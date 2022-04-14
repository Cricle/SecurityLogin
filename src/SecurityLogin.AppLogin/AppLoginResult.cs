using System;

namespace SecurityLogin.AppLogin
{
    public class AppLoginResult
    {
        public AppLoginCode Code { get; set; }

        public string AccessToken { get; set; }

        public TimeSpan? ExpireTime { get; set; }

        public DateTime CreateAt { get; set; }
    }
}
