using StackExchange.Redis;

namespace SecurityLogin.Cache
{
    public static class CacheOperatorWriteExtensions
    {
        public static T Create<T>(this ComplexCacheOperator @operator, HashEntry[] entries)
        {
            return (T)Create(@operator,entries);
        }
        public static object Create(this ComplexCacheOperator @operator, HashEntry[] entries)
        {
            var inst = @operator.Create();
            @operator.Write(ref inst, entries);
            return inst;
        }
        public static void Write<T>(this ICacheOperator @operator,ref T instance, HashEntry[] entries)
        {
            object val = instance;
            @operator.Write(ref val, entries);
            instance = (T)val;
        }
    }
}
