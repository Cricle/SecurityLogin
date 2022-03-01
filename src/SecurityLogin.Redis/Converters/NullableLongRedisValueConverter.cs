using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableLongRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableLongRedisValueConverter Instance = new NullableLongRedisValueConverter();

        private NullableLongRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (long?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value.HasValue)
            {
                return null;
            }
            if (value.TryParse(out long val))
            {
                return val;
            }
            return null;
        }
    }
}
