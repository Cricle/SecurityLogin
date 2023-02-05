using Microsoft.AspNetCore.Http;

namespace SecurityLogin.AspNetCore
{
    public static class HttpFetchExtensions
    {
        public static string GetFromHeaderOrCookie(this HttpContext context, string key)
        {
            var accessToken = context.Request.Headers[key];
            if (accessToken.Count == 0)
            {
                return null;
            }
            var accessTk = accessToken.ToString();
            if (string.IsNullOrEmpty(accessTk))
            {
                context.Request.Cookies.TryGetValue(key, out accessTk);
            }
            return accessTk;
        }
    }
}
