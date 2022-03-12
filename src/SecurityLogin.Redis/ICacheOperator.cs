using StackExchange.Redis;

namespace SecurityLogin.Cache
{
    public interface ICacheOperator
    {
        void Build();

        void Write(ref object instance, HashEntry[] entries);

        HashEntry[] As(object value);
    }
}
