using System;
using System.Security.Cryptography;

namespace SecurityLogin.Mode.AES
{
    public partial class AESFullKey : AsymmetricFullKey, IIdentityable
    {
        public string Identity { get; set; }
    }
    public partial class AESFullKey
    {
        public byte[] IV { get; set; }

        public PaddingMode PaddingMode { get; set; }

        public CipherMode CipherMode { get; set; }

        public byte[] Key { get; set; }

        public int KeySize { get; set; }
    }
}
