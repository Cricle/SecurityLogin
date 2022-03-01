using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class DoubleRedisValueConverter : IRedisValueConverter
    {
        public static readonly DoubleRedisValueConverter Instance = new DoubleRedisValueConverter();

        private DoubleRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (string)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            return value.ToString();
        }
    }
}
