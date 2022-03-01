using System.Text;

namespace SecurityLogin.Mode.RSA
{
    public class RSAEncryptor<TFullKey> : Base64Encryptor<TFullKey>
           where TFullKey : AsymmetricFullKey
    {
        public static readonly RSAEncryptor<TFullKey> SharedUTF8 = new RSAEncryptor<TFullKey>(Encoding.UTF8);

        public RSAEncryptor(Encoding encoding) : base(encoding)
        {
        }

        public override byte[] Decrypt(TFullKey fullKey, byte[] data)
        {
            return Helpers.RSA.DecryptByPrivateKey(data, fullKey.PrivateKey);
        }

        public override byte[] Encrypt(TFullKey fullKey, byte[] data)
        {
            return Helpers.RSA.EncryptByPublicKey(data, fullKey.PublicKey);
        }
    }
}
