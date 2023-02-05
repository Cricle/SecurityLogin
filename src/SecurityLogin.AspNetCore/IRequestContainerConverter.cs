using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public interface IRequestContainerConverter<TRequestContainer>
    {
        Task<TRequestContainer> ConvertAsync(HttpContext context);
    }
}
