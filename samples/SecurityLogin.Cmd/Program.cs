using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SecurityLogin.Mode.RSA;
using SecurityLogin.Mode.RSA.Helpers;
using SecurityLogin.Store.Memory;

namespace SecurityLogin.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run().GetAwaiter().GetResult();
        }
        private static async Task Run()
        {
            var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var cv = new MemoryCacheVisitor(cache);
            var fc = new MemoryLockFactory();
            var ser = new MyLoginService(fc, cv);
            var res = await ser.FlushKeyAsync();

            Console.WriteLine(res.PublicKey);
            Console.WriteLine();

            var d=Console.ReadLine();
            Console.WriteLine(await ser.LoginAsync(res.Identity,d));
        }
    }
    internal class MyLoginService : RSALoginService<AsymmetricFullKey>
    {
        public MyLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor) : base(lockerFactory, cacheVisitor)
        {
        }

        public MyLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor, IEncryptor<AsymmetricFullKey> encryptor) : base(lockerFactory, cacheVisitor, encryptor)
        {
        }

        protected override AsymmetricFullKey GetFullKey()
        {
            return RSAHelper.GenerateRSASecretKey();
        }

        protected override string GetHeader()
        {
            return "My";
        }

        protected override string GetSharedIdentityKey()
        {
            return "My.Shared";
        }

        protected override string GetSharedLockKey()
        {
            return "My.Lock";
        }

        public Task<string> LoginAsync(string connectId,string pwd)
        {
            return DecryptAsync(connectId, pwd);
        }
    }
}