using System;

namespace SecurityLogin.AppLogin.Models
{
    public interface IAppSession
    {
        string AppKey { get; }

        string Token { get; }

        DateTime CreateTime { get; }
    }
}
