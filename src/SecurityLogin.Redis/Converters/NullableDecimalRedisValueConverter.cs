using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableDecimalRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableDecimalRedisValueConverter Instance = new NullableDecimalRedisValueConverter();

        private NullableDecimalRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (double?)((decimal?)value);
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (decimal?)value;
        }
    }
}
