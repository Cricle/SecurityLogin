namespace SecurityLogin
{
    public interface IEncryptable<TFullKey>
    {
        IEncryptor<TFullKey> Encryptor { get; }
    }
}
