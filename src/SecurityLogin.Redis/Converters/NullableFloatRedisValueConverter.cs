using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableFloatRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableFloatRedisValueConverter Instance = new NullableFloatRedisValueConverter();

        private NullableFloatRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (float?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value.HasValue)
            {
                return null;
            }
            return (float?)value;
        }
    }
}
