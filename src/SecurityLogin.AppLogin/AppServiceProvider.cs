namespace SecurityLogin.AppLogin
{
    public class AppServiceProvider : IAppServiceProvider
    {
        public AppServiceProvider(IKeyGenerator keyGenerator, ITimeHelper timeHelper, ICacheVisitor cacheVisitor, IEncryptionHelper encryptionHelper)
        {
            KeyGenerator = keyGenerator ?? throw new System.ArgumentNullException(nameof(keyGenerator));
            TimeHelper = timeHelper ?? throw new System.ArgumentNullException(nameof(timeHelper));
            CacheVisitor = cacheVisitor ?? throw new System.ArgumentNullException(nameof(cacheVisitor));
            EncryptionHelper = encryptionHelper ?? throw new System.ArgumentNullException(nameof(encryptionHelper));
        }

        public IKeyGenerator KeyGenerator { get; }

        public ITimeHelper TimeHelper { get; }

        public ICacheVisitor CacheVisitor { get; }

        public IEncryptionHelper EncryptionHelper { get; }
    }
}
