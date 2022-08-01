using Ao.Cache;
using Ao.Cache.InMemory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SecurityLogin.Mode.AES;
using SecurityLogin.Mode.AES.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SecurityLogin.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(a);
            //Console.WriteLine(b);
            Run().GetAwaiter().GetResult();
        }
        private static async Task Run()
        {
            var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var cv = new MemoryCacheVisitor(cache);
            var fc = new MemoryLockFactory();
            var ser = new MyLoginService(fc, cv);
            var res = await ser.FlushKeyAsync();

            Console.WriteLine("IV:"+Convert.ToBase64String(res.IV));
            Console.WriteLine("Key:" + Convert.ToBase64String(res.Key));
            var buf = AESEncryptor<AESFullKey>.SharedUTF8.Encrypt(res, Encoding.UTF8.GetBytes("world"));
            Console.WriteLine(Convert.ToBase64String(buf));
            Console.WriteLine(Encoding.UTF8.GetString(AESEncryptor<AESFullKey>.SharedUTF8.Decrypt(res, buf)));
            Console.WriteLine();
            var d = Console.ReadLine();
            Console.WriteLine(await ser.LoginAsync(res.Identity, d));
        }
    }


    internal class MyLoginService : AESLoginService
    {
        public MyLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor) : base(lockerFactory, cacheVisitor)
        {
        }

        public MyLoginService(ILockerFactory lockerFactory, ICacheVisitor cacheVisitor, IEncryptor<AESFullKey> encryptor) : base(lockerFactory, cacheVisitor, encryptor)
        {
        }

        public Task<string> LoginAsync(string connectId, string pwd)
        {
            return DecryptAsync(connectId, pwd);
        }
    }
}


