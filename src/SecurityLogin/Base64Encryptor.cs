using System;
using System.Text;

namespace SecurityLogin
{
    public abstract class Base64Encryptor<TFullKey> : IEncryptor<TFullKey>
    {
        protected Base64Encryptor(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public Encoding Encoding { get; }

        public abstract byte[] Decrypt(TFullKey fullKey, byte[] data);

        public virtual byte[] Decrypt(TFullKey fullKey, string data)
        {
            return Decrypt(fullKey, Convert.FromBase64String(data));
        }

        public virtual string DecryptToString(TFullKey fullKey, byte[] data)
        {
            var bs = Decrypt(fullKey, data);
            return Encoding.GetString(bs, 0, bs.Length);
        }

        public virtual string DecryptToString(TFullKey fullKey, string data)
        {
            return DecryptToString(fullKey, Convert.FromBase64String(data));
        }

        public abstract byte[] Encrypt(TFullKey fullKey, byte[] data);

        public virtual byte[] Encrypt(TFullKey fullKey, string data)
        {
            return Encrypt(fullKey, Encoding.GetBytes(data));
        }

        public virtual string EncryptToString(TFullKey fullKey, string data)
        {
            return EncryptToString(fullKey, Encoding.GetBytes(data));
        }

        public virtual string EncryptToString(TFullKey fullKey, byte[] data)
        {
            var res = Encrypt(fullKey, data);
            return Convert.ToBase64String(res);
        }
    }
}
