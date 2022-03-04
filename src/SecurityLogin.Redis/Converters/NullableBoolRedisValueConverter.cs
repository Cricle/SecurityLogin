using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableBoolRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableBoolRedisValueConverter Instance = new NullableBoolRedisValueConverter();

        private NullableBoolRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (bool?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (bool?)value;
        }
    }
}
