using Ao.ObjectDesign;
using StackExchange.Redis;

namespace SecurityLogin.Redis
{
    public static class RedisOperatorWriteExtensions
    {
        public static T Create<T>(this ComplexRedisOperator @operator, HashEntry[] entries)
        {
            return (T)Create(@operator,entries);
        }
        public static object Create(this ComplexRedisOperator @operator, HashEntry[] entries)
        {
            var inst = ReflectionHelper.Create(@operator.Target);
            @operator.Write(ref inst, entries);
            return inst;
        }
        public static void Write<T>(this IRedisOperator @operator,ref T instance,HashEntry[] entries)
        {
            object val = instance;
            @operator.Write(ref val, entries);
            instance = (T)val;
        }
    }
    public interface IRedisOperator
    {
        void Build();

        void Write(ref object instance, HashEntry[] entries);

        HashEntry[] As(object value);
    }
}
