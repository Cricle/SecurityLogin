using Ao.Cache;
using System;
using System.Threading.Tasks;

namespace SecurityLogin
{
    public abstract class AsymmetricLoginService<TFullKey> : SecurityLoginService<TFullKey>, IEncryptable<TFullKey>
           where TFullKey : AsymmetricFullKey
    {
        public IEncryptor<TFullKey> Encryptor { get; }

        protected AsymmetricLoginService(ILockerFactory lockerFactory,
            ICacheVisitor cacheVisitor,
            IEncryptor<TFullKey> encryptor)
            : base(lockerFactory, cacheVisitor)
        {
            Encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
        }
        public Task<string> DecryptAsync(string connectId, string textHash)
        {
            return DecryptAsync(connectId, textHash, Encryptor);
        }
    }
}
