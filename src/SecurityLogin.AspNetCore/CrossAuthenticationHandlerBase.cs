using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SecurityLogin.AccessSession;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddDefaultSecurityLoginHandler<TInput,TUserSnapshot>(this IServiceCollection services)
            where TUserSnapshot : UserSnapshot
        {
            services.AddSingleton<IRequestContainerConverter<UserStatusContainer<TUserSnapshot>>,DefaultRequestContainerConverter<TUserSnapshot, TInput>>();
            services.AddScoped<CrossAuthenticationHandler<TUserSnapshot>>();
            services.AddSingleton<RequestContainerOptions>();
            return services;
        }
    }
    public abstract class CrossAuthenticationHandlerBase<TRequestContainer>:IAuthenticationHandler
    {
        private HttpContext context;
        private AuthenticationScheme scheme;

        protected CrossAuthenticationHandlerBase(IRequestContainerConverter<TRequestContainer> requestContainerConverter)
        {
            RequestContainerConverter = requestContainerConverter ?? throw new ArgumentNullException(nameof(requestContainerConverter));
        }

        public AuthenticationScheme AuthenticationScheme => scheme;

        public HttpContext HttpContext => context;

        public IRequestContainerConverter<TRequestContainer> RequestContainerConverter { get; }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            if (!await IsSkipAuthAsync())
            {
                var container = await RequestContainerConverter.ConvertAsync(context);
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

        public virtual Task ChallengeAsync(AuthenticationProperties properties)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public virtual Task ForbidAsync(AuthenticationProperties properties)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }
        public virtual Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            this.context = context;
            this.scheme = scheme;
            return Task.CompletedTask;
        }

        protected abstract Task<bool> IsSkipAuthAsync();
    }
}
