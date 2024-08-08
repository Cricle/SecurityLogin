using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecurityLogin.AppLogin;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    internal sealed class AppLoginMiddleware<TAppInfoSnapshot> : IMiddleware
        where TAppInfoSnapshot:IAppInfoSnapshot
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var options = context.RequestServices.GetRequiredService<IOptions<AppLoginOptions>>().Value;
            var intercepts = context.RequestServices.GetServices<IAppLoginIntercept<TAppInfoSnapshot>>();
            var loginProvider = context.RequestServices.GetRequiredService<IAppLoginProvider>();

            foreach (var intercept in intercepts)
            {
                await intercept.BeforeAsync(context);
            }

            var needToCheck = await loginProvider.NeedToCheckAsync(context);

            if (needToCheck)
            {
                var appKey = loginProvider.GetAppKey(context);
                if (!string.IsNullOrEmpty(appKey))
                {
                    var snapshotProvider = context.RequestServices.GetRequiredService<IAppInfoSnapshotProvider<TAppInfoSnapshot>>();
                    var snapshot = await snapshotProvider.GetAppInfoSnapshotAsync(appKey);
                    if (snapshot != null)
                    {
                        context.Features.Set(snapshot);
                    }
                    else
                    {
                        await loginProvider.AppSnatsnopNotFoundHandlerAsync(context, appKey);
                        return;
                    }
                }
                else
                {
                    await loginProvider.AppKeyEmptyHandlerAsync(context);
                    return;
                }
            }


            foreach (var intercept in intercepts)
            {
                await intercept.AfterAsync(context);
            }
            await next(context);
        }
    }
}
