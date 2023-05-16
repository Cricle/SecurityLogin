using Ao.Cache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SecurityLogin.AccessSession;
using SecurityLogin.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SecurityLoginAddOptions<TUserSnapshot>
    {
        public string? SchemeDisplayDescript { get; set; } = "se-default";

        public Func<RequestContainerOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>>? ContainerOptionsFunc { get; set; }
    }
    public static class SecurityLoginAddExtensions
    {
        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<TInput, TUserSnapshot>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            string getKeyPrefx,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null)
        {
            AddSecurityLogin<TInput, TUserSnapshot>(services, optionsFun);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p =>
                new DelegateIdentityService<TInput, TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    tk => KeyGenerator.Concat(getKeyPrefx, tk),
                    generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<TInput, TUserSnapshot, THandler>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            string getKeyPrefx,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null)
                          where THandler : IAuthenticationHandler
        {
            AddSecurityLogin<TInput, TUserSnapshot,THandler>(services, optionsFun);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p =>
                new DelegateIdentityService<TInput, TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    tk => KeyGenerator.Concat(getKeyPrefx, tk),
                    generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLoginCustomWithDefaultIdentity<TInput, TUserSnapshot>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            string getKeyPrefx,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, AuthenticationOptions>? configurateAuthenticationOptions = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, AuthorizationOptions>? configurateAuthorizationOptions = null)
        {
            AddSecurityLoginCustom<TInput, TUserSnapshot>(services, optionsFun, configurateAuthenticationOptions, configurateAuthorizationOptions);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p =>
                new DelegateIdentityService<TInput, TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    tk => KeyGenerator.Concat(getKeyPrefx, tk),
                    generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<TInput, TUserSnapshot>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            IdentityGetKeyHandler? getKeyHandler = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null)
        {
            AddSecurityLogin<TInput, TUserSnapshot>(services, optionsFun);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p =>
                new DelegateIdentityService<TInput, TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    getKeyHandler, generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLoginCustomWithDefaultIdentity<TInput, TUserSnapshot>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            IdentityGetKeyHandler? getKeyHandler = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, AuthenticationOptions>? configurateAuthenticationOptions = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, AuthorizationOptions>? configurateAuthorizationOptions = null)
        {
            AddSecurityLoginCustom<TInput, TUserSnapshot>(services, optionsFun,configurateAuthenticationOptions, configurateAuthorizationOptions);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p =>
                new DelegateIdentityService<TInput, TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    getKeyHandler, generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<TInput, TUserSnapshot, THandler>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            IdentityGetKeyHandler? getKeyHandler = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null)
                   where THandler : IAuthenticationHandler
        {
            AddSecurityLogin<TInput, TUserSnapshot, THandler>(services, optionsFun);
            services.AddScoped<IIdentityService<TInput, TUserSnapshot>>(p =>
                new DelegateIdentityService<TInput, TUserSnapshot>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    getKeyHandler, generateTokenHandler));
            return services;
        }
        public static IServiceCollection AddSecurityLogin<TInput, TUserSnapshot,THandler>(this IServiceCollection services,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null)
             where THandler : IAuthenticationHandler
        {
            return AddSecurityLoginCustom<TInput, TUserSnapshot>(services,
                optionsFun, (opt, x) =>
                {
                    x.DefaultAuthenticateScheme = SecurityLoginConsts.AuthenticationScheme;
                    x.AddScheme<THandler>(SecurityLoginConsts.AuthenticationScheme, opt.SchemeDisplayDescript ?? "se-default");
                });
        }
        public static IServiceCollection AddSecurityLogin<TInput, TUserSnapshot>(this IServiceCollection services,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null)
        {
            return AddSecurityLoginCustom<TInput, TUserSnapshot>(services,
                optionsFun, (opt, x) =>
                {
                    x.DefaultAuthenticateScheme = SecurityLoginConsts.AuthenticationScheme;
                    x.AddScheme<CrossAuthenticationHandler<TUserSnapshot>>(SecurityLoginConsts.AuthenticationScheme, opt.SchemeDisplayDescript ?? "se-default");
                });
        }
        public static IServiceCollection AddSecurityLoginCustom<TInput, TUserSnapshot>(this IServiceCollection services,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, AuthenticationOptions>? configurateAuthenticationOptions = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, AuthorizationOptions>? configurateAuthorizationOptions = null)
        {
            var opt = new SecurityLoginAddOptions<TUserSnapshot>();
            optionsFun?.Invoke(opt);
            services.AddDefaultSecurityLoginHandler<TInput, TUserSnapshot>();
            services.AddAuthorization(x =>
            {
                configurateAuthorizationOptions?.Invoke(opt, x);
            });
            services.AddAuthentication(x =>
            {
                configurateAuthenticationOptions?.Invoke(opt, x);
            });
            var containerOpt = new RequestContainerOptions<TUserSnapshot>();
            containerOpt = opt.ContainerOptionsFunc?.Invoke(containerOpt) ?? containerOpt;
            services.AddSingleton(containerOpt);
            return services;
        }
    }
}
