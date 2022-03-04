using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableUIntRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableUIntRedisValueConverter Instance = new NullableUIntRedisValueConverter();

        private NullableUIntRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (uint?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (uint?)value;
        }
    }
}
