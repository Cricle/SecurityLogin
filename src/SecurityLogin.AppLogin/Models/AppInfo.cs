using System;

namespace SecurityLogin.AppLogin.Models
{
    public class AppInfo : IAppInfo
    {
        public string AppKey { get; set; }

        public string AppSecret { get; set; }

        public DateTime? EndTime { get; set; }

        public bool Enable { get; set; }
    }
}
