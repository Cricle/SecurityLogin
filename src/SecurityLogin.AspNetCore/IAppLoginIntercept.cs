using Microsoft.AspNetCore.Http;
using SecurityLogin.AppLogin;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public interface IAppLoginIntercept<TAppInfoSnapshot>
          where TAppInfoSnapshot : IAppInfoSnapshot
    {
        Task BeforeAsync(HttpContext context);

        Task AfterAsync(HttpContext context);

        Task SucceedAsync(HttpContext context,TAppInfoSnapshot snapshot);

        Task FailAsync(HttpContext context);
    }
}
