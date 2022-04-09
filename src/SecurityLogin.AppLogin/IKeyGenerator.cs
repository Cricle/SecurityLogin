using System.Threading.Tasks;

namespace SecurityLogin.AppLogin
{
    public interface IKeyGenerator
    {
        Task<string> GenSecretKeyAsync();
    }
}
