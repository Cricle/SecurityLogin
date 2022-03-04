using StackExchange.Redis;
using System;

namespace SecurityLogin.Redis.Converters
{
    public class NullableDateTimeRedisValueConverter : IRedisValueConverter
    {
        public static readonly NullableDateTimeRedisValueConverter Instance = new NullableDateTimeRedisValueConverter();

        private NullableDateTimeRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            var dt = (DateTime?)value;
            if (dt == null)
            {
                return RedisValue.EmptyString;
            }
            return dt.Value.Ticks;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            if (value.TryParse(out long tick))
            {
                return new DateTime(tick);
            }
            return null;
        }
    }
}
