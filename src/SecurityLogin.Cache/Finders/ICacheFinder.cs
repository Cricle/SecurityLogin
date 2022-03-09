using System.Threading.Tasks;

namespace SecurityLogin.Cache.Finders
{
    public interface ICacheFinder<TIdentity, TEntity>
    {
        Task<TEntity> FindInDbAsync(TIdentity identity, bool cache = true);
        
        Task<bool> SetInCahceAsync(TIdentity identity,TEntity entity);
        
        Task<TEntity> FindInCahceAsync(TIdentity identity);
    }
}
