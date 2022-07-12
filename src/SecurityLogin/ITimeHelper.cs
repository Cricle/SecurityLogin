using System;

namespace SecurityLogin
{
    public interface ITimeHelper
    {
        long GetTimeStamp(DateTime time);

        DateTime ToDateTime(long timestamp);
    }
}
