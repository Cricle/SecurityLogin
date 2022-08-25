using Microsoft.IO;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SecurityLogin.Mode.AES
{
    public class AESEncryptor<TFullKey> : Base64Encryptor<TFullKey>
        where TFullKey : AESFullKey
    {
        private static readonly RecyclableMemoryStreamManager streamManager = new RecyclableMemoryStreamManager();

        public static readonly AESEncryptor<TFullKey> SharedUTF8 = new AESEncryptor<TFullKey>(Encoding.UTF8);
        public AESEncryptor(Encoding encoding) : base(encoding)
        {
        }

        public override byte[] Decrypt(TFullKey fullKey, byte[] data)
        {
            using (var aes = Aes.Create())
            {
                InitAes(fullKey, aes);
                using (var mem = streamManager.GetStream(data))
                using (var cs = new CryptoStream(mem, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
                {
                    return Read(cs);
                }
            }
        }
        private static byte[] Read(Stream stream)
        {
            var res = new List<byte>();
            var buffer = ArrayPool<byte>.Shared.Rent(2048);
            try
            {
                var read = 0;
                while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    if (buffer.Length == read)
                    {
                        res.AddRange(buffer);
                    }
                    else
                    {
                        res.AddRange(buffer.Take(read));
                    }
                }
                return res.ToArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        public override byte[] Encrypt(TFullKey fullKey, byte[] data)
        {
            using (var aes = Aes.Create())
            {
                InitAes(fullKey, aes);
                using (var mem = streamManager.GetStream())
                {
                    using (var cs = new CryptoStream(mem, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                        return mem.ToArray();
                    }
                }
            }
        }
        protected virtual void InitAes(TFullKey fullKey, Aes aes)
        {
            aes.KeySize = fullKey.KeySize;
            aes.Padding = fullKey.PaddingMode;
            aes.Mode = fullKey.CipherMode;
            aes.Key = fullKey.Key;
            aes.IV = fullKey.IV;
        }
    }
}