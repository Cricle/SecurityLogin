using System;
using System.Security.Cryptography;
using System.Text;

namespace SecurityLogin.Mode.AES
{
    public class AESEncryptor<TFullKey> : Base64Encryptor<TFullKey>
        where TFullKey : AESFullKey
    {
        public static readonly AESEncryptor<TFullKey> SharedUTF8 = new AESEncryptor<TFullKey>(Encoding.UTF8);
        public AESEncryptor(Encoding encoding) : base(encoding)
        {
        }

        public override byte[] Decrypt(TFullKey fullKey, byte[] data)
        {
            using (var aes = new RijndaelManaged())
            {
                InitAES(aes);
                aes.Key = Convert.FromBase64String(fullKey.Key);
                using (var transform = aes.CreateDecryptor())
                {
                    var res = transform.TransformFinalBlock(data, 0, data.Length);
                    return res;
                }
            }
        }
        public override byte[] Encrypt(TFullKey fullKey, byte[] data)
        {
            using (var aes = new RijndaelManaged())
            {
                InitAES(aes);
                aes.Key = Convert.FromBase64String(fullKey.Key);
                using (var transform = aes.CreateEncryptor())
                {
                    return transform.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }
        protected virtual void InitAES(RijndaelManaged aes)
        {
            AESIniter.InitAES(aes);
        }
    }
    internal static class AESIniter
    {
        public static void InitAES(RijndaelManaged aes)
        {
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.BlockSize = 128;
        }
    }
}
