using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public class UIntRedisValueConverter : IRedisValueConverter
    {
        public static readonly UIntRedisValueConverter Instance = new UIntRedisValueConverter();

        private UIntRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (uint)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (uint)value;
        }
    }
}
