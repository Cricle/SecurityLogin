using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class IntRedisValueConverter : IRedisValueConverter
    {
        public static readonly IntRedisValueConverter Instance = new IntRedisValueConverter();

        private IntRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (int)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value .TryParse(out int val))
            {
                return val;
            }
            return default(int);
        }
    }
}
