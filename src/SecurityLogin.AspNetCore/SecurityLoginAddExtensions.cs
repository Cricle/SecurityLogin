using Ao.Cache;
using SecurityLogin.AccessSession;
using SecurityLogin.AspNetCore;
using System;
using System.Security.Cryptography;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SecurityLoginAddOptions
    {
        public string? SchemeDisplayDescript { get; set; } = "se-default";

        public Func<RequestContainerOptions, RequestContainerOptions>? ContainerOptionsFunc { get; set; }
    }
    public static class SecurityLoginAddExtensions
    {
        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<TInput, TUserSnapshot>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            string getKeyPrefx,
            Action<SecurityLoginAddOptions>? optionsFun=null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null)
        {
            AddSecurityLogin<TInput, TUserSnapshot>(services, optionsFun);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p =>
                new DelegateIdentityService<TInput, TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    tk=>KeyGenerator.Concat(getKeyPrefx, tk),
                    generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<TInput, TUserSnapshot>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            IdentityGetKeyHandler? getKeyHandler = null,
            Action<SecurityLoginAddOptions>? optionsFun=null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null)
        {
            AddSecurityLogin<TInput,TUserSnapshot>(services, optionsFun);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p=>
                new DelegateIdentityService<TInput,TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    getKeyHandler,generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLogin<TInput, TUserSnapshot>(this IServiceCollection services, Action<SecurityLoginAddOptions>? optionsFun = null)
        {
            var opt = new SecurityLoginAddOptions();
            optionsFun?.Invoke(opt);
            services.AddDefaultSecurityLoginHandler<TInput, TUserSnapshot>();
            services.AddAuthorization();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = SecurityLoginConsts.AuthenticationScheme;
                x.AddScheme<CrossAuthenticationHandler<TUserSnapshot>>(SecurityLoginConsts.AuthenticationScheme, opt.SchemeDisplayDescript ?? "se-default");
            });
            var containerOpt = new RequestContainerOptions();
            containerOpt = opt.ContainerOptionsFunc?.Invoke(containerOpt) ?? containerOpt;
            services.AddSingleton(containerOpt);
            return services;
        }
    }
}
