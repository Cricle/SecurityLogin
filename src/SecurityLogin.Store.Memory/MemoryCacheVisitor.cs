using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Store.Memory
{
    public class MemoryCacheVisitor : ICacheVisitor
    {
        public MemoryCacheVisitor(IMemoryCache cache)
        {
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public IMemoryCache Cache { get; }

        public bool Delete(string key)
        {
            Cache.Remove(key);
            return true;
        }

        public Task<bool> DeleteAsync(string key)
        {            
            return Task.FromResult(Delete(key));
        }

        public bool Exists(string key)
        {
            return Cache.TryGetValue(key, out _);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(Cache.TryGetValue(key, out _));
        }

        public bool Expire(string key, TimeSpan? cacheTime)
        {
            var val = Cache.Get(key);
            if (val == null)
            {
                return false;
            }
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = cacheTime
            };
            Cache.Set(key, val, options);
            return true;
        }

        public Task<bool> ExpireAsync(string key, TimeSpan? cacheTime)
        {
            return Task.FromResult(Expire(key, cacheTime));
        }

        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        public string GetString(string key)
        {
            return Get<string>(key);
        }

        public Task<string> GetStringAsync(string key)
        {
           return Task.FromResult(GetString(key));
        }

        public bool Set<T>(string key, T value, TimeSpan? cacheTime)
        {
            if (cacheTime.HasValue)
            {
                Cache.Set(key, value, cacheTime.Value);
            }
            else
            {
                Cache.Set(key, value);
            }
            return true;
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? cacheTime)
        {
            Set(key, value, cacheTime);
            return Task.FromResult(true);
        }

        public bool SetString(string key, string value, TimeSpan? cacheTime)
        {
           return Set(key, value, cacheTime);
        }

        public Task<bool> SetStringAsync(string key, string value, TimeSpan? cacheTime)
        {
            return SetAsync(key, value, cacheTime);
        }
    }
}
