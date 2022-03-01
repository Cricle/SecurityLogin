using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;

namespace SecurityLogin.Mode.RSA.Helpers
{
    public static class RSAHelper
    {
        public const int RSAKeyLen = 2048;
        public static AsymmetricFullKey GenerateRSASecretKey()
        {
            return GenerateRSASecretKey(RSAKeyLen);
        }
        public static AsymmetricFullKey GenerateRSASecretKey(int keyLen)
        {
            var key= RSA.GetKey(keyLen);
            return new AsymmetricFullKey
            {
                Identity = Guid.NewGuid().ToString(),
                PrivateKey = key.PrivateKey,
                PublicKey = key.PublicKey,
            };
		}
    }
}
