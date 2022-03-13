using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Finders
{
    public interface IBatchCacheFinder<TIdentity, TEntity>
    {
        Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IEnumerable<TIdentity> identity, bool cache = true);
       
        Task<bool> SetInCahceAsync(IEnumerable<KeyValuePair<TIdentity, TEntity>> pairs);
       
        Task<IDictionary<TIdentity, TEntity>> FindInCahceAsync(IEnumerable<TIdentity> identity);
    }
}
