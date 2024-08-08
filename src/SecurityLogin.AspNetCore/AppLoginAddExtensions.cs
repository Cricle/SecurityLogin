using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SecurityLogin.AppLogin;
using System;

namespace SecurityLogin.AspNetCore
{
    public static class AppLoginAddExtensions
    {
        public static IServiceCollection AddAppLoginDefaultProvider(this IServiceCollection services, AppLoginOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.AddSingleton(options.CreateProvider());
            return services;
        }
        public static IServiceCollection AddAppLogin<TAppInfoSnapshotProvider>(this IServiceCollection services, Action<AppLoginOptions>? config = null, ServiceLifetime appServiceProviderLifetime = ServiceLifetime.Scoped)
            where TAppInfoSnapshotProvider:class,IAppInfoSnapshotProvider<IAppInfoSnapshot>
        {
            return AddAppLogin<TAppInfoSnapshotProvider, IAppInfoSnapshot>(services, config, appServiceProviderLifetime);
        }
        public static IServiceCollection AddAppLogin<TAppInfoSnapshotProvider, TAppInfoSnapshot>(this IServiceCollection services, Action<AppLoginOptions>? config = null, ServiceLifetime appServiceProviderLifetime = ServiceLifetime.Scoped)
            where TAppInfoSnapshotProvider : class, IAppInfoSnapshotProvider<TAppInfoSnapshot>
            where TAppInfoSnapshot : IAppInfoSnapshot
        {
            if (config != null)
            {
                services.Configure(config);
            }
            services.AddScoped<IAppServiceProvider, AppServiceProvider>()
                .AddSingleton<IKeyGenerator, DefaultKeyGenerator>()
                .AddSingleton<IRandomProvider>(DefaultRandomProvider.Instance)
                .Add(ServiceDescriptor.Describe(typeof(IAppInfoSnapshotProvider<IAppInfoSnapshot>), typeof(TAppInfoSnapshotProvider), appServiceProviderLifetime));

            services.AddSingleton<AppLoginMiddleware<TAppInfoSnapshot>>();

            return services;
        }
        public static IApplicationBuilder UseAppLogin(this IApplicationBuilder builder) 
        {
            return UseAppLogin<IAppInfoSnapshot>(builder);
        }
        public static IApplicationBuilder UseAppLogin<TAppInfoSnapshot>(this IApplicationBuilder builder)
            where TAppInfoSnapshot : IAppInfoSnapshot
        {
            builder.UseMiddleware<AppLoginMiddleware<TAppInfoSnapshot>>();
            return builder;
        }
    }
}
