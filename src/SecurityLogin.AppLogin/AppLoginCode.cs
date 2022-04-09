using System;

namespace SecurityLogin.AppLogin
{
    [Flags]
    public enum AppLoginCode
    {
        Succeed = 0,
        AppEndOfTime = 1,
        SignError = 2,
        NoSuchAppKey = 3,
        TimestampOutOfTime = 4,
    }
}
