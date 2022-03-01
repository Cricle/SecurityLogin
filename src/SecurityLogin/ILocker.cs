using System;

namespace SecurityLogin
{
    public interface ILocker: IDisposable
    {
        string Resource { get; }

        string LockId { get; }

        bool IsAcquired { get; }

        DateTime CreateTime { get; }

        TimeSpan ExpireTime { get; }
    }
}
