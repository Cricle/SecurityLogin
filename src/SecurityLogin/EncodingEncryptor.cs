using System;
using System.Text;

namespace SecurityLogin
{
    public abstract class EncodingEncryptor<TFullKey> : IEncryptor<TFullKey>
    {
        protected EncodingEncryptor(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public Encoding Encoding { get; }

        public abstract byte[] Decrypt(TFullKey fullKey, byte[] data);

        public byte[] Decrypt(TFullKey fullKey, string data)
        {
            return Decrypt(fullKey, Encoding.GetBytes(data));
        }

        public string DecryptToString(TFullKey fullKey, byte[] data)
        {
            var bs = Decrypt(fullKey, data);
            return Encoding.GetString(bs, 0, bs.Length);
        }

        public string DecryptToString(TFullKey fullKey, string data)
        {
            return DecryptToString(fullKey, Encoding.GetBytes(data));
        }

        public abstract byte[] Encrypt(TFullKey fullKey, byte[] data);

        public byte[] Encrypt(TFullKey fullKey, string data)
        {
            return Encrypt(fullKey, Encoding.GetBytes(data));
        }

        public string EncryptToString(TFullKey fullKey, string data)
        {
            return EncryptToString(fullKey, Encoding.GetBytes(data));
        }

        public string EncryptToString(TFullKey fullKey, byte[] data)
        {
            var res = Encrypt(fullKey, data);
            return Encoding.GetString(res, 0, res.Length);
        }
    }
}
