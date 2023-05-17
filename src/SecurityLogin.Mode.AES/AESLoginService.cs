using Ao.Cache;
using SecurityLogin.Mode.AES.Helpers;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.Mode.AES
{
    public abstract class AESLoginService : AESLoginService<AESFullKey>
    {
        protected AESLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor) : base(lockerFactory, cacheVisitor)
        {
        }

        protected AESLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor, IEncryptor<AESFullKey> encryptor) : base(lockerFactory, cacheVisitor, encryptor)
        {
        }

        protected override AESFullKey GetFullKey()
        {
            return AESHelper.CreateKey();
        }
    }
    public abstract class AESLoginService<TFullKey> : SecurityLoginService<TFullKey>, IEncryptable<TFullKey>
        where TFullKey : AESFullKey
    {
        protected AESLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor, IEncryptor<TFullKey> encryptor)
            : base(lockerFactory, cacheVisitor)
        {
            Encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
        }
        protected AESLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor)
            : this(lockerFactory, cacheVisitor, AESEncryptor<TFullKey>.SharedUTF8)
        {
        }

        public IEncryptor<TFullKey> Encryptor { get; }

        public Task<string?> DecryptAsync(string connectId, string textHash)
        {
            return DecryptAsync(connectId, textHash, Encryptor);
        }
    }
}
