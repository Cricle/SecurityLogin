using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class DoubleRedisValueConverter : IRedisValueConverter
    {
        public static readonly DoubleRedisValueConverter Instance = new DoubleRedisValueConverter();

        private DoubleRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (double)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (double)value;
        }
    }

    public class CharRedisValueConverter : IRedisValueConverter
    {
        public static readonly CharRedisValueConverter Instance = new CharRedisValueConverter();

        private CharRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (long)(char)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (char)(long)value;
        }
    }
}
