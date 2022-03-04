using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class ShortRedisValueConverter : IRedisValueConverter
    {
        public static readonly ShortRedisValueConverter Instance = new ShortRedisValueConverter();

        private ShortRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (long)(short)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (short)(long)value;
        }
    }
}
