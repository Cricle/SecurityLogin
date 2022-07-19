using System;

namespace SecurityLogin
{
    [Flags]
    public enum CacheSetIf
    {
        Always = 0,
        Exists = 1,
        NotExists = 2
    }
}
