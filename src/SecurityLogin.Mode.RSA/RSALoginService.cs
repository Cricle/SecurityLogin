using System;
using System.Collections.Generic;
using System.Text;
using SecurityLogin.Mode.RSA.Helpers;

namespace SecurityLogin.Mode.RSA
{
    public abstract class RSALoginService<TFullKey> : AsymmetricLoginService<TFullKey>
        where TFullKey :AsymmetricFullKey
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
