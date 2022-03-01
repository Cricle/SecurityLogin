using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class DecimalRedisValueConverter : IRedisValueConverter
    {
        public static readonly DecimalRedisValueConverter Instance = new DecimalRedisValueConverter();

        private DecimalRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (double)((decimal)value);
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (decimal)value;
        }
    }
}
