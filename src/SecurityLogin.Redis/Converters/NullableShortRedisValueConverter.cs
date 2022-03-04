using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableShortRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableShortRedisValueConverter Instance = new NullableShortRedisValueConverter();

        private NullableShortRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (short?)(char?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (short?)(int?)value;
        }
    }
}
