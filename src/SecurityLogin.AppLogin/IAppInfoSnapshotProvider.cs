using System.Threading.Tasks;

namespace SecurityLogin.AppLogin
{
    public interface IAppInfoSnapshotProvider<TAppInfoSnapshot>
        where TAppInfoSnapshot : IAppInfoSnapshot
    {
        Task<TAppInfoSnapshot> GetAppInfoSnapshotAsync(string appKey);
    }
}
