using SecurityLogin.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SecurityLoginAddExtensions
    {
        public static IServiceCollection AddSecurityLogin<T>(this IServiceCollection services, Func<RequestContainerOptions, RequestContainerOptions> optFunc = null, string descript = null)
        {
            services.AddAuthorization();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = SecurityLoginConsts.AuthenticationScheme;
                x.AddScheme<CrossAuthenticationHandler<T>>(SecurityLoginConsts.AuthenticationScheme, descript ?? "se-default");
            });
            var opt = new RequestContainerOptions();
            opt = optFunc?.Invoke(opt) ?? opt;
            services.AddSingleton(opt);
            return services;
        }
    }
}
