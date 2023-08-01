using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecurityLogin.AccessSession;
using System;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddDefaultSecurityLoginHandler<TInput,TUserSnapshot>(this IServiceCollection services)
        {
            services.AddSingleton<IRequestContainerConverter<UserStatusContainer<TUserSnapshot>>,DefaultRequestContainerConverter<TUserSnapshot, TInput>>();
            services.AddScoped<CrossAuthenticationHandler<TUserSnapshot>>();
            return services;
        }
    }
    internal static class Throws
    {
        public static void ThrowHttpContextIsNull(HttpContext? context)
        {
            if (context == null)
            {
                throw new InvalidOperationException("The HttpContext is null");
            }
        }
    }
    public abstract class CrossAuthenticationHandlerBase<TRequestContainer>: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected CrossAuthenticationHandlerBase(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            IRequestContainerConverter<TRequestContainer> requestContainerConverter)
            :base(options,logger,encoder,clock)
        {
            RequestContainerConverter = requestContainerConverter ?? throw new ArgumentNullException(nameof(requestContainerConverter));
        }

        public IRequestContainerConverter<TRequestContainer> RequestContainerConverter { get; }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Throws.ThrowHttpContextIsNull(Context);
            if (!await IsSkipAuthAsync())
            {
                var container = await RequestContainerConverter.ConvertAsync(Context);
                try
                {
                    return await CheckAsync();
                }
                finally
                {
                    if (container is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            return await SkipAsync();
        }
        protected abstract Task<AuthenticateResult> CheckAsync();

        protected abstract Task<AuthenticateResult> SkipAsync();

        protected abstract Task<bool> IsSkipAuthAsync();
    }
}
