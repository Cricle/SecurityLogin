using System;
using System.Threading.Tasks;

namespace SecurityLogin
{
    public interface ICacheVisitor
    {
        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        bool Set<T>(string key, T value, TimeSpan? cacheTime);

        Task<bool> SetAsync<T>(string key,T value, TimeSpan? cacheTime);

        string GetString(string key);

        Task<string> GetStringAsync(string key);

        bool SetString(string key, string value, TimeSpan? cacheTime);
        
        Task<bool> SetStringAsync(string key, string value, TimeSpan? cacheTime);

        bool Delete(string key);

        Task<bool> DeleteAsync(string key);
    }
}
