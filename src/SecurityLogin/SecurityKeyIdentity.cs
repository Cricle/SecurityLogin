using System;

namespace SecurityLogin
{
    public class SecurityKeyIdentity:IEquatable<SecurityKeyIdentity>, IIdentityable
    {
        public SecurityKeyIdentity(string identity, string key)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public string Identity { get; }

        public string Key { get; }

        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17 * 31 + Identity.GetHashCode();
                h = 31 * h + Key.GetHashCode();
                return h;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SecurityKeyIdentity);
        }
        public bool Equals(SecurityKeyIdentity other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Identity==Identity && other.Key==Key;
        }
        public override string ToString()
        {
            return  $"{{Identity: {Identity}, Key: {Key}}}";
        }
    }
}
