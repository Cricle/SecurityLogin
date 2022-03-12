using StackExchange.Redis;

namespace SecurityLogin.Cache
{
    public interface IListCacheOperator
    {
        void Build();

        void Write(ref object instance, RedisValue[] entries);

        RedisValue[] As(object value);
    }
    public interface IHashCacheOperator
    {
        void Build();

        void Write(ref object instance, HashEntry[] entries);

        HashEntry[] As(object value);
    }
}
