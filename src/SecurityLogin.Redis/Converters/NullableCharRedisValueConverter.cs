using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableCharRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableCharRedisValueConverter Instance = new NullableCharRedisValueConverter();

        private NullableCharRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (int?)(char?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (char?)(int?)value;
        }
    }
}
