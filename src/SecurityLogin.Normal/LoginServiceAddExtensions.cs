using Ao.Cache;
using Ao.Cache.InRedis;
using RedLockNet;
using SecurityLogin;
using StackExchange.Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoginServiceAddExtensions
    {
        public static IServiceCollection AddNormalSecurityService(this IServiceCollection services, Action<LoginServiceOptions>? action = null)
        {
            var opt = new LoginServiceOptions();
            action?.Invoke(opt);
            services.AddSingleton<ILockerFactory>(x => new RedisLockFactory(x.GetRequiredService<IDistributedLockFactory>()));
            services.AddSingleton<ICacheVisitor, RedisCacheVisitor>();
            services.AddSingleton<ITimeHelper>(DefaultTimeHelper.Default);
            services.AddSingleton<IEncryptionHelper>(Md5EncryptionHelper.Instance);
            return services;
        }
    }
}
