using Microsoft.Extensions.Options;
using RedLockNet;
using SecurityLogin;
using SecurityLogin.Store.Redis;
using SecurityLogin.Transfer.TextJson;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoginServiceAddExtensions
    {
        public static IServiceCollection AddNormalSecurityService(this IServiceCollection services,Action<LoginServiceOptions> action=null)
        {
            var opt = new LoginServiceOptions();
            action?.Invoke(opt);
            services.AddSingleton<ILockerFactory>(x => new RedisLockFactory(x.GetRequiredService<IDistributedLockFactory>()));
            services.AddSingleton<IObjectTransfer>(new JsonObjectTransfer(opt.JsonOptions, opt.Encoding));
            services.AddSingleton<ICacheVisitor>(x => new RedisCacheVisitor(x.GetRequiredService<IConnectionMultiplexer>().GetDatabase(), x.GetRequiredService<IObjectTransfer>()));
            return services;
        }
    }
}
