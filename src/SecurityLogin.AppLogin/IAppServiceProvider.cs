namespace SecurityLogin.AppLogin
{
    public interface IAppServiceProvider
    {
        IKeyGenerator KeyGenerator { get; }

        ITimeHelper TimeHelper { get; }

        ICacheVisitor CacheVisitor { get; }

        IEncryptionHelper EncryptionHelper { get; }
    }
}
