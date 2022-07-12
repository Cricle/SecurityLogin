using System.Threading.Tasks;

namespace SecurityLogin.AccessSession
{
    public interface IIdentityService<TInput, TTokenInfo>
    {
        Task<bool> DeleteAsync(string token);

        Task<bool> ExistsAsync(string token);

        Task<TTokenInfo> GetTokenInfoAsync(string token);

        Task<string> IssureTokenAsync(TInput input);

        Task<bool> RenewAsync(string token);
    }
}