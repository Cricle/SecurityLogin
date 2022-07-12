using System;

namespace SecurityLogin
{
    public class HeaderGenerator
    {
        public HeaderGenerator(string identity)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        }

        public string Identity { get; }

        public string GetHeader()
        {
            return "SecurityLogin.Auto." + Identity;
        }

        public string GetSharedIdentityKey()
        {
            return "SecurityLogin.Auto.SharedIdentity." + Identity;
        }

        public string GetSharedLockKey()
        {
            return "Lock.SecurityLogin.Auto." + Identity;
        }
    }
}
