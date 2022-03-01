namespace SecurityLogin
{
    public interface IEncryptor<TFullKey>
    {
        byte[] Encrypt(TFullKey fullKey,byte[] data);

        byte[] Encrypt(TFullKey fullKey, string data);

        string EncryptToString(TFullKey fullKey, string data);

        string EncryptToString(TFullKey fullKey, byte[] data);

        byte[] Decrypt(TFullKey fullKey, byte[] data);

        string DecryptToString(TFullKey fullKey, byte[] data);

        byte[] Decrypt(TFullKey fullKey, string data);

        string DecryptToString(TFullKey fullKey, string data);
    }
}
