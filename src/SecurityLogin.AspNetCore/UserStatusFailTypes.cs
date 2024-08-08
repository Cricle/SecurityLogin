using System;

namespace SecurityLogin.AspNetCore
{
    [Flags]
    public enum UserStatusFailTypes
    {
        NoAppLogin=0,
        NoUserSnapshot=1
    }
}
