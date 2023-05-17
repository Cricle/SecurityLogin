using System;

namespace SecurityLogin.AppLogin
{
    public class AppInfoSnapshot : IAppInfoSnapshot
    {
        public string? AppSecret { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
