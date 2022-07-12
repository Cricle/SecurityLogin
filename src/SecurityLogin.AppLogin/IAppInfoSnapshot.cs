using System;

namespace SecurityLogin.AppLogin
{
    public interface IAppInfoSnapshot
    {
        string AppSecret { get; }

        DateTime? EndTime { get; }
    }
}
