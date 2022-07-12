using System;

namespace SecurityLogin
{
    public class DefaultTimeHelper : ITimeHelper
    {
        public static readonly DefaultTimeHelper Default = new DefaultTimeHelper();

        public DefaultTimeHelper() { }

        private static readonly DateTime zeroTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public long GetTimeStamp(DateTime time)
        {
            var t = time.Kind != DateTimeKind.Utc ? time.ToUniversalTime() : time;
            return (long)(t - zeroTime).TotalMilliseconds;
        }
        public static long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow - zeroTime).TotalMilliseconds;
        }

        public DateTime ToDateTime(long timestamp)
        {
            return zeroTime.AddMilliseconds(timestamp).ToLocalTime();
        }
    }
}
