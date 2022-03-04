using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Finders
{
    public static class BatchCacheFinderExtensions
    {
        public static async Task<TEntity> FindAsync<TIdentity, TEntity>(this ICacheFinder<TIdentity,TEntity> finder, TIdentity identity, bool cache = true)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder));
            }
            var val = await finder.FindInCahceAsync(identity).ConfigureAwait(false);
            if (default(TEntity).Equals(val)) 
            {
                return await finder.FindInDbAsync(identity, cache).ConfigureAwait(false);
            }
            return val;
        }
        public static async Task<IReadOnlyDictionary<TIdentity, TEntity>> FindAsync<TIdentity,TEntity>(this IBatchCacheFinder<TIdentity,TEntity> finder, IEnumerable<TIdentity> identities, bool cache = true)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder));
            }

            if (identities is null)
            {
                throw new ArgumentNullException(nameof(identities));
            }

            var cacheDatas = await finder.FindInCahceAsync(identities).ConfigureAwait(false);
            var notIncludes = identities.Except(cacheDatas.Keys);
            if (!notIncludes.Any())
            {
                return cacheDatas;
            }
            var dbDatas = await finder.FindInDbAsync(notIncludes, cache).ConfigureAwait(false);
            var res = new Dictionary<TIdentity, TEntity>(cacheDatas.Count + dbDatas.Count);
            foreach (var item in cacheDatas)
            {
                res[item.Key] = item.Value;
            }
            foreach (var item in dbDatas)
            {
                res[item.Key] = item.Value;
            }

            return res;
        }
    }
}
