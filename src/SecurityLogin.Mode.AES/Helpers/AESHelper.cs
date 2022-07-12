using System;
using System.Security.Cryptography;

namespace SecurityLogin.Mode.AES.Helpers
{
    public static class AESHelper
    {
        public static AESFullKey CreateKey()
        {
            using (var aes = new RijndaelManaged())
            {
                AESIniter.InitAES(aes);
                aes.GenerateIV();
                aes.GenerateKey();
                return new AESFullKey
                {
                    Identity = Guid.NewGuid().ToString(),
                    IV = Convert.ToBase64String(aes.IV),
                    Key = Convert.ToBase64String(aes.Key)
                };
            }
        }
    }
}
