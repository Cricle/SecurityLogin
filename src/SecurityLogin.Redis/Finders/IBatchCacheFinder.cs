using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Finders
{
    public interface IBatchCacheFinder<TIdentity, TEntity>
    {
        Task<IReadOnlyDictionary<TIdentity, TEntity>> FindInDbAsync(IEnumerable<TIdentity> identity, bool cache = true);
       
        Task<bool> SetInCahceAsync(IReadOnlyDictionary<TIdentity, TEntity> pairs);
       
        Task<IReadOnlyDictionary<TIdentity, TEntity>> FindInCahceAsync(IEnumerable<TIdentity> identity);
    }
}
