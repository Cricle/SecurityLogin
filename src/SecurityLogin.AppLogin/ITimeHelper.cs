using System;

namespace SecurityLogin.AppLogin
{
    public interface ITimeHelper
    {
        long GetTimeStamp(in DateTime time);

        DateTime ToDateTime(long timestamp);
    }
}
