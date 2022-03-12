using StackExchange.Redis;

namespace SecurityLogin.Cache
{
    public interface IEntryCacheOperator
    {
        void Build();

        void Write(ref object instance, RedisValue entry);

        RedisValue As(object value);
    }
}
