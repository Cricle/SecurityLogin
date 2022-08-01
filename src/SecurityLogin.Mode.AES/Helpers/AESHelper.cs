using System;
using System.Security.Cryptography;

namespace SecurityLogin.Mode.AES.Helpers
{
    public static class AESHelper
    {
        public static AESFullKey CreateKey(PaddingMode paddingMode= PaddingMode.PKCS7,CipherMode mode= CipherMode.ECB)
        {
            using (var aes = Aes.Create())
            {
                aes.Padding = paddingMode;
                aes.Mode = mode;
                aes.GenerateIV();
                aes.GenerateKey();
                return new AESFullKey
                {
                    Identity = Guid.NewGuid().ToString(),
                    IV = aes.IV,
                    Key = aes.Key,
                    PaddingMode = aes.Padding,
                    CipherMode = mode,
                    KeySize = aes.KeySize,
                };
            }
        }
    }
}
