using Ao.Cache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SecurityLogin.AccessSession;
using SecurityLogin.AppLogin.Models;
using SecurityLogin.AspNetCore;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SecurityLoginAddOptions<TUserSnapshot>
    {
        public string? SchemeDisplayDescript { get; set; } = "se-default";

        public Func<RequestContainerOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>>? ContainerOptionsFunc { get; set; }

        public Action<SecurityLoginAddOptions<TUserSnapshot>, AuthenticationOptions>? ConfigurateAuthenticationOptions { get; set; }

        public Action<SecurityLoginAddOptions<TUserSnapshot>, AuthorizationOptions>? ConfigurateAuthorizationOptions { get; set; }
    }
    public static class SecurityLoginAddExtensions
    {
        internal static T Set<T>(this T val,Action<T> action)
        {
            action(val);
            return val;
        }
        public static IServiceCollection AddAppLoginDefaultSessionManager(this IServiceCollection services,
            string getKeyPrefx="SecurityLogin:AppSession",
            IdentityGenerateTokenHandler<AppSession>? generateTokenHandler = null)
        {
            return AddAppLoginDefaultSessionManager(services, req => Task.FromResult(req.Input.Set(x => x.Token = req.Token)), getKeyPrefx, generateTokenHandler);
        }
        public static IServiceCollection AddAppLoginDefaultSessionManager<TInput,TAppSession>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TAppSession> asTokenInfoHandler,
            string getKeyPrefx = "SecurityLogin:UserSession",
            IdentityGenerateTokenHandler<TInput>? generateTokenHandler = null)
        {
            services.AddScoped<IIdentityService<TInput, TAppSession>>(p =>
                new DelegateIdentityService<TInput, TAppSession>(
                    p.GetRequiredService<ICacheVisitor>(),
                    asTokenInfoHandler,
                    tk => KeyGenerator.Concat(getKeyPrefx, tk),
                    generateTokenHandler));
            return services;
        }

        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<TInput, TUserSnapshot>(this IServiceCollection services,
            IdentityAsTokenInfoHandler<TInput, TUserSnapshot> asTokenInfoHandler,
            string getKeyPrefx = "SecurityLogin:UserSession",
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
        public static IServiceCollection AddSecurityLoginWithDefaultIdentity<THandler>(this IServiceCollection services,
            string getKeyPrefx = "SecurityLogin:UserSession",
            string authenticationScheme = "SecurityLogin",
            string authHeader = "SecurityLogin",
            Action<SecurityLoginAddOptions<UserSnapshot>>? optionsFun = null,
            IdentityGenerateTokenHandler<UserSnapshot>? generateTokenHandler = null)
                where THandler : IAuthenticationHandler
        {
            return AddSecurityLoginWithDefaultIdentity<UserSnapshot, UserSnapshot, THandler>(services,
                req => Task.FromResult(req.Input.Set(x => x.Token = req.Token)),
                getKeyPrefx,
                options =>
                {
                    options.ContainerOptionsFunc = opt =>
                    {
                        opt.AuthenticationScheme = authenticationScheme;
                        opt.AuthHeader = authHeader;
                        return opt;
                    };
                    optionsFun?.Invoke(options);
                }, 
                generateTokenHandler);
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
            Action<SecurityLoginAddOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>, AuthenticationOptions>? configurateAuthenticationOptions = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>, AuthorizationOptions>? configurateAuthorizationOptions = null)
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
            Action<SecurityLoginAddOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>, AuthenticationOptions>? configurateAuthenticationOptions = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>, AuthorizationOptions>? configurateAuthorizationOptions = null)
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
                optionsFun, (opt,copt, x) =>
                {
                    x.AddScheme<THandler>(copt.AuthenticationScheme, opt.SchemeDisplayDescript ?? "se-default");
                });
        }
        public static IServiceCollection AddSecurityLogin<TInput, TUserSnapshot>(this IServiceCollection services,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null)
        {
            services.AddScoped<CrossAuthenticationHandler<TUserSnapshot>>();
            return AddSecurityLogin<TInput, TUserSnapshot, CrossAuthenticationHandler<TUserSnapshot>>(services,
                optionsFun);
        }
        public static IServiceCollection AddSecurityLoginCustom<TInput, TUserSnapshot>(this IServiceCollection services,
            Action<SecurityLoginAddOptions<TUserSnapshot>>? optionsFun = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>, AuthenticationOptions>? configurateAuthenticationOptions = null,
            Action<SecurityLoginAddOptions<TUserSnapshot>, RequestContainerOptions<TUserSnapshot>, AuthorizationOptions>? configurateAuthorizationOptions = null)
        {
            var opt = new SecurityLoginAddOptions<TUserSnapshot>();
            optionsFun?.Invoke(opt);
            var containerOpt = new RequestContainerOptions<TUserSnapshot>();
            containerOpt = opt.ContainerOptionsFunc?.Invoke(containerOpt) ?? containerOpt;
            services.AddSingleton(containerOpt);
            services.AddDefaultSecurityLoginHandler<TInput, TUserSnapshot>();
            services.AddAuthorization(x =>
            {
                opt.ConfigurateAuthorizationOptions?.Invoke(opt, x);
                configurateAuthorizationOptions?.Invoke(opt!, containerOpt, x);
            });
            services.AddAuthentication(x =>
            {
                x.DefaultForbidScheme = containerOpt.AuthenticationScheme;
                x.DefaultAuthenticateScheme = containerOpt.AuthenticationScheme;
                x.DefaultScheme = containerOpt.AuthenticationScheme;
                x.DefaultChallengeScheme = containerOpt.AuthenticationScheme;
                opt.ConfigurateAuthenticationOptions?.Invoke(opt, x);
                configurateAuthenticationOptions?.Invoke(opt,containerOpt, x);
            });
            return services;
        }
    }
}
