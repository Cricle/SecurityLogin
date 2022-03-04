using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class ULongRedisValueConverter : IRedisValueConverter
    {
        public static readonly ULongRedisValueConverter Instance = new ULongRedisValueConverter();

        private ULongRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (ulong)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (ulong)value;
        }
    }
}
