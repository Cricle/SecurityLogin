using StackExchange.Redis;
using System;

namespace SecurityLogin.Redis.Converters
{

    public class ByteArrayRedisValueConverter : IRedisValueConverter
    {
        public static readonly ByteArrayRedisValueConverter Instance = new ByteArrayRedisValueConverter();

        private ByteArrayRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (byte[])value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (byte[])value;
        }
    }
}
