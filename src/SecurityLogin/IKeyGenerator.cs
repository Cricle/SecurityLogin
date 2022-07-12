using System.Threading.Tasks;

namespace SecurityLogin
{
    public interface IKeyGenerator
    {
        Task<string> GenSecretKeyAsync();
    }
}
