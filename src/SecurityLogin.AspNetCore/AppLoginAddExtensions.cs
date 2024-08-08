using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SecurityLogin.AccessSession;
using SecurityLogin.AppLogin;
using SecurityLogin.AppLogin.Models;
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
        public static IServiceCollection AddAppLogin<TAppInfoSnapshotProvider>(this IServiceCollection services,
            AppLoginOptions? options=null,
            string getKeyPrefx = "SecurityLogin:AppSession",
            IdentityGenerateTokenHandler<AppSession>? generateTokenHandler = null,
            ServiceLifetime appServiceProviderLifetime = ServiceLifetime.Scoped,
            ServiceLifetime appInfoSnapshotProviderLifetime= ServiceLifetime.Scoped)
            where TAppInfoSnapshotProvider : class, IAppInfoSnapshotProvider<IAppInfoSnapshot>
        {
            options ??= new AppLoginOptions();
            services.TryAdd(ServiceDescriptor.Describe(typeof(TAppInfoSnapshotProvider), typeof(TAppInfoSnapshotProvider), appInfoSnapshotProviderLifetime));

            AddAppLoginServices<TAppInfoSnapshotProvider>(services, appServiceProviderLifetime);

            AddAppLoginDefaultProvider(services, options);

            services.AddAppLoginDefaultSessionManager(getKeyPrefx, generateTokenHandler);

            return services;
        }

        public static IServiceCollection AddAppLoginServices<TAppInfoSnapshotProvider>(this IServiceCollection services, ServiceLifetime appServiceProviderLifetime = ServiceLifetime.Scoped)
            where TAppInfoSnapshotProvider : class, IAppInfoSnapshotProvider<IAppInfoSnapshot>
        {
            return AddAppLoginServices<AppSession, AppSession, TAppInfoSnapshotProvider, IAppInfoSnapshot>(services, appServiceProviderLifetime);
        }
        public static IServiceCollection AddAppLoginServices<TInput, TAppSession, TAppInfoSnapshotProvider>(this IServiceCollection services, ServiceLifetime appServiceProviderLifetime = ServiceLifetime.Scoped)
            where TAppInfoSnapshotProvider:class,IAppInfoSnapshotProvider<IAppInfoSnapshot>
        {
            return AddAppLoginServices<TInput, TAppSession, TAppInfoSnapshotProvider, IAppInfoSnapshot>(services, appServiceProviderLifetime);
        }
        public static IServiceCollection AddAppLoginServices<TInput,TAppSession,TAppInfoSnapshotProvider, TAppInfoSnapshot>(this IServiceCollection services, ServiceLifetime appServiceProviderLifetime = ServiceLifetime.Scoped)
            where TAppInfoSnapshotProvider : class, IAppInfoSnapshotProvider<TAppInfoSnapshot>
            where TAppInfoSnapshot : IAppInfoSnapshot
        {
            services.AddScoped<IAppServiceProvider, AppServiceProvider>()
                .AddSingleton<IKeyGenerator, DefaultKeyGenerator>()
                .AddSingleton<IRandomProvider>(DefaultRandomProvider.Instance)
                .Add(ServiceDescriptor.Describe(typeof(IAppInfoSnapshotProvider<IAppInfoSnapshot>), typeof(TAppInfoSnapshotProvider), appServiceProviderLifetime));

            services.AddSingleton<AppLoginMiddleware<TInput,TAppSession,TAppInfoSnapshot>>();

            return services;
        }
        public static IApplicationBuilder UseAppLogin(this IApplicationBuilder builder) 
        {
            return UseAppLogin<AppSession, AppSession, IAppInfoSnapshot>(builder);
        }
        public static IApplicationBuilder UseAppLogin<TInput, TAppSession, TAppInfoSnapshot>(this IApplicationBuilder builder)
            where TAppInfoSnapshot : IAppInfoSnapshot
        {
            builder.UseMiddleware<AppLoginMiddleware<TInput, TAppSession, TAppInfoSnapshot>>();
            return builder;
        }
    }
}
