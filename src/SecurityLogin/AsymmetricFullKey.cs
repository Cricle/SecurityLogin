using System;

namespace SecurityLogin
{
    public class AsymmetricFullKey : IIdentityable
    {
        public string? Identity { get; set; }

        public string? PublicKey { get; set; }

        public string? PrivateKey { get; set; }

        public void ThrowIfIdentityNull()
        {
            if (Identity==null)
            {
                throw new ArgumentNullException(nameof(Identity));
            }
        }
        public void ThrowIfIPublicKeyNull()
        {
            if (PublicKey == null)
            {
                throw new ArgumentNullException(nameof(PublicKey));
            }
        }
        public void ThrowIfIPrivateKeyNull()
        {
            if (PrivateKey == null)
            {
                throw new ArgumentNullException(nameof(PrivateKey));
            }
        }
    }
}
