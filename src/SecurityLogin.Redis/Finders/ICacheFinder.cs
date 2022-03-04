using Ao.ObjectDesign;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Finders
{
    public interface ICacheFinder<TIdentity, TEntity>
    {
        Task<TEntity> FindInDbAsync(TIdentity identity, bool cache = true);
        
        Task<bool> SetInCahceAsync(TIdentity identity,TEntity entity);
        
        Task<TEntity> FindInCahceAsync(TIdentity identity);
    }
}
