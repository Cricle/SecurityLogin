using Ao.Cache;
using Ao.Cache.InMemory;
using Microsoft.Extensions.Caching.Memory;
using SecurityLogin;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoginServiceAddExtensions
    {
        public static IServiceCollection AddInMemorySecurityService(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ILockerFactory>(x => new MemoryLockFactory());
            services.AddSingleton<ICacheVisitor>(x =>
            {
                var db = x.GetService<IMemoryCache>();
                return new MemoryCacheVisitor(db);
            });
            services.AddSingleton<ITimeHelper>(DefaultTimeHelper.Default);
            services.AddSingleton<IEncryptionHelper>(Md5EncryptionHelper.Instance);
            return services;
        }
    }
}
