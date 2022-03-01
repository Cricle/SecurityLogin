using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class FloatRedisValueConverter : IRedisValueConverter
    {
        public static readonly FloatRedisValueConverter Instance = new FloatRedisValueConverter();

        private FloatRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (float)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (float)value;
        }
    }
}
