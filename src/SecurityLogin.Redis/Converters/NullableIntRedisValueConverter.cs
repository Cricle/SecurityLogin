using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class NullableIntRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableIntRedisValueConverter Instance = new NullableIntRedisValueConverter();

        private NullableIntRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (int?)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value .HasValue)
            {
                return null; 
            }
            if (value.TryParse(out int val))
            {
                return val;
            }
            return null;
        }
    }
}
