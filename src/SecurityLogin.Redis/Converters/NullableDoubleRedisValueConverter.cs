using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableDoubleRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableDoubleRedisValueConverter Instance = new NullableDoubleRedisValueConverter();

        private NullableDoubleRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (double?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            if (value.TryParse(out double val))
            {
                return val;
            }
            return null;
        }
    }
}
