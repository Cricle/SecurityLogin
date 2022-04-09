using System;

namespace SecurityLogin.AppLogin
{
    public class AppServiceOptions
    {
        public TimeSpan RequestTimestampOffset { get; set; } = TimeSpan.FromMinutes(5);

        public TimeSpan AppSessionTime { get; set; } = TimeSpan.FromHours(6);
    }
}
