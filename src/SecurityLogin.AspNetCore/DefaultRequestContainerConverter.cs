using Microsoft.AspNetCore.Http;
using SecurityLogin.AccessSession;
using SecurityLogin.AppLogin;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SecurityLogin.AspNetCore
{
    public class DefaultRequestContainerConverter<TUserSnapshot, TInput> : IRequestContainerConverter<UserStatusContainer<TUserSnapshot>>
    {
        public async Task<UserStatusContainer<TUserSnapshot>> ConvertAsync(HttpContext context)
        {
            var options = context.RequestServices.GetRequiredService<IOptions<RequestContainerOptions<TUserSnapshot>>>();

            var container = new UserStatusContainer<TUserSnapshot>();

            var opt = options.Value;

            if (!opt.NoAppLogin)
            {
                var appKey = context.GetFromHeaderOrCookie(opt.APPKeyHeader);
                var accessToken = context.GetFromHeaderOrCookie(opt.AccessHeader);
                if (appKey != null && accessToken != null)
                {
                    var appSer = context.RequestServices.GetRequiredService<AppService>();
                    var hasSession = await appSer.HasSessionAsync(appKey, accessToken);
                    if (hasSession)
                    {
                        container.AppSnapshot = new AppSnapshot { AppKey = appKey, AppSession = accessToken };
                    }
                }
            }
            if (!opt.NoUserCheck && (opt.NoAppLogin||container.HasAppSnapshot || opt.AppFailNoUser))
            {
                var authToken = context.GetFromHeaderOrCookie(opt.AuthHeader);
                if (authToken != null)
                {
                    var identitySer = context.RequestServices.GetRequiredService<IIdentityService<TInput, TUserSnapshot>>();
                    var tk = await identitySer.GetTokenInfoAsync(authToken);
                    container.UserSnapshot = tk;
                }
            }
            return container;
        }
    }
}
