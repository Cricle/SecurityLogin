using StackExchange.Redis;

namespace SecurityLogin.Redis
{
    public interface IRedisOperator
    {
        void Build();

        void Write(ref object instance, HashEntry[] entries);

        HashEntry[] As(object value);
    }
}
