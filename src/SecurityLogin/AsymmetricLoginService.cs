using System;
using System.Threading.Tasks;

namespace SecurityLogin
{
    public abstract class AsymmetricLoginService<TFullKey> : SecurityLoginService<TFullKey>
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
        protected override SecurityKeyIdentity Transfer(TFullKey fullKey)
        {
            return new SecurityKeyIdentity(fullKey.Identity,fullKey.PublicKey);
        }
        protected async Task<string> DecryptAsync(string connectId, string textHash)
        {
            var header = GetHeader();
            var fullKey= await GetFullKeyAsync(header, connectId);
            if (fullKey is null)
            {
                return null;
            }
            return Encryptor.DecryptToString(fullKey, textHash);
        }
    }
}
