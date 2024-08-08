using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public interface IAppLoginProvider
    {
        ValueTask<bool> NeedToCheckAsync(HttpContext context);

        Task AppKeyEmptyHandlerAsync(HttpContext context);

        Task AppSnatsnopNotFoundHandlerAsync(HttpContext context,string appKey);

        string? GetAppKey(HttpContext context);
    }
}
