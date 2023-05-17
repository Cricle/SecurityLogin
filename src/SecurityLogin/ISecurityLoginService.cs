using System.Threading.Tasks;

namespace SecurityLogin
{
    public interface ISecurityLoginService<TFullKey> where TFullKey : IIdentityable
    {
        Task<string?> DecryptAsync(string connectId, string textHash, IEncryptor<TFullKey> encrypter);
        Task<TFullKey?> FlushKeyAsync();
    }
}