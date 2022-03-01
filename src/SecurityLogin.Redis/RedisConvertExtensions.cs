using SecurityLogin.Redis.Converters;
using System;

namespace StackExchange.Redis
{
    public static class RedisConvertExtensions
    {
        public static T Get<T>(this in RedisValue value)
        {
            return (T)Get(value, typeof(T));
        }
        public static object Get(this in RedisValue value,Type type)
        {
            var convert = KnowsRedisValueConverter.GetConverter(type);
            return convert?.ConvertBack(value, null);
        }
    }
}
