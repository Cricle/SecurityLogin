using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class LongRedisValueConverter : IRedisValueConverter
    {
        public static readonly LongRedisValueConverter Instance = new LongRedisValueConverter();

        private LongRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (long)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value.TryParse(out long val))
            {
                return val;
            }
            return default(long);
        }
    }
}
