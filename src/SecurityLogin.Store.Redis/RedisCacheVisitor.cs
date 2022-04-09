using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Store.Redis
{
    public class RedisCacheVisitor : ICacheVisitor
    {
        public RedisCacheVisitor(IDatabase database, IObjectTransfer objectTransfer)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            ObjectTransfer = objectTransfer ?? throw new ArgumentNullException(nameof(objectTransfer));
        }

        public IDatabase Database { get; }

        public IObjectTransfer ObjectTransfer { get; }

        public bool Delete(string key)
        {
            return Database.KeyDelete(key);            
        }

        public Task<bool> DeleteAsync(string key)
        {
            return Database.KeyDeleteAsync(key);
        }

        public bool Exists(string key)
        {
            return Database.KeyExists(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Database.KeyExistsAsync(key);
        }

        public T Get<T>(string key)
        {
            var val = Database.StringGet(key);
            if (val.HasValue)
            {
                return ObjectTransfer.Transfer<T>(val);
            }
            return default;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var val =await Database.StringGetAsync(key);
            if (val.HasValue)
            {
                return ObjectTransfer.Transfer<T>(val);
            }
            return default;
        }

        public string GetString(string key)
        {
            return Database.StringGet(key);
        }

        public async Task<string> GetStringAsync(string key)
        {
            var res = await Database.StringGetAsync(key);
            return res;
        }

        public bool Set<T>(string key, T value, TimeSpan? cacheTime)
        {
            var buffer=ObjectTransfer.Transfer(value);
            return Database.StringSet(key, buffer, cacheTime);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? cacheTime)
        {
            var buffer = ObjectTransfer.Transfer(value);
            return Database.StringSetAsync(key, buffer, cacheTime);
        }

        public bool SetString(string key, string value, TimeSpan? cacheTime)
        {
            return Database.StringSet(key, value, cacheTime);
        }

        public Task<bool> SetStringAsync(string key, string value, TimeSpan? cacheTime)
        {
            return Database.StringSetAsync(key, value, cacheTime);
        }
    }
}
