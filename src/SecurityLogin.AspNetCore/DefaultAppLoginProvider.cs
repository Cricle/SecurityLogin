using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    internal sealed class DefaultAppLoginProvider : IAppLoginProvider
    {
        private readonly AppLoginOptions options;

        public DefaultAppLoginProvider(AppLoginOptions options)
        {
            this.options = options;
        }

        public Task AppKeyEmptyHandlerAsync(HttpContext context)
        {
            context.Response.StatusCode = options.AppKeyEmptyStatusCode;
            if (string.IsNullOrEmpty(options.AppKeyEmptyResponseMsg))
            {
                return Task.CompletedTask;
            }
            return context.Response.WriteAsync(options.AppKeyEmptyResponseMsg);
        }

        public Task AppSnatsnopNotFoundHandlerAsync(HttpContext context, string appKey)
        {
            context.Response.StatusCode = options.AppSnatshopEmptyStatusCode;
            if (string.IsNullOrEmpty(options.AppSnatshopEmptyResponseMsg))
            {
                return Task.CompletedTask;
            }
            return context.Response.WriteAsync(options.AppSnatshopEmptyResponseMsg);
        }

        public string? GetAppKey(HttpContext context)
        {
            return context.GetFromHeaderOrCookie(options.AppKeyHeader);
        }

        public ValueTask<bool> NeedToCheckAsync(HttpContext context)
        {
            foreach (var item in options.NotNeedToCheck)
            {
                if (context.Request.Path.StartsWithSegments(item, StringComparison.OrdinalIgnoreCase))
                {
                    return new ValueTask<bool>(false);
                }
            }
            return new ValueTask<bool>(true);
        }
    }
}
