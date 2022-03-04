using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class ByteRedisValueConverter : IRedisValueConverter
    {
        public static readonly ByteRedisValueConverter Instance = new ByteRedisValueConverter();

        private ByteRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (long)(byte)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (byte)(long)value;
        }
    }
}
