using Ao.Cache;
using SecurityLogin.Mode.RSA.Helpers;

namespace SecurityLogin.Mode.RSA
{
    public abstract class RSALoginService : RSALoginService<AsymmetricFullKey>
    {
        protected RSALoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor) : base(lockerFactory, cacheVisitor)
        {
        }

        protected RSALoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor, IEncryptor<AsymmetricFullKey> encryptor) : base(lockerFactory, cacheVisitor, encryptor)
        {
        }
        protected override AsymmetricFullKey GetFullKey()
        {
            return RSAHelper.GenerateRSASecretKey();
        }
        protected virtual int GetKeyLength()
        {
            return RSAHelper.RSAKeyLen;
        }
    }
    public abstract class RSALoginService<TFullKey> : AsymmetricLoginService<TFullKey>
        where TFullKey : AsymmetricFullKey
    {
        protected RSALoginService(ILockerFactory lockerFactory,
               ICacheVisitor cacheVisitor)
               : base(lockerFactory, cacheVisitor, RSAEncryptor<TFullKey>.SharedUTF8)
        {
        }

        protected RSALoginService(ILockerFactory lockerFactory,
            ICacheVisitor cacheVisitor,
            IEncryptor<TFullKey> encryptor)
            : base(lockerFactory, cacheVisitor, encryptor)
        {
        }
    }
}
