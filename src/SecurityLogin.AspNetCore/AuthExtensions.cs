using Microsoft.Extensions.DependencyInjection;
using SecurityLogin.AccessSession;

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
}
