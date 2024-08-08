using System;

namespace SecurityLogin.AppLogin.Models
{
    public class AppSession : IAppSession
    {
        public string AppKey { get; set; } = null!;

        public string Token { get; set; } = null!;

        public DateTime CreateTime { get; set; }
    }
}
