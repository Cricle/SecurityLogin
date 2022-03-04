using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableULongRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableULongRedisValueConverter Instance = new NullableULongRedisValueConverter();

        private NullableULongRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (ulong?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (ulong?)value;
        }
    }
}
