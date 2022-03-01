using StackExchange.Redis;
using System;

namespace SecurityLogin.Redis.Converters
{
    public class DateTimeRedisValueConverter : IRedisValueConverter
    {
        public static readonly DateTimeRedisValueConverter Instance = new DateTimeRedisValueConverter();

        private DateTimeRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            var dt = (DateTime)value;
            return dt.Ticks;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            if (value.TryParse(out long tick))
            {
                return new DateTime(tick);
            }
            return RedisValueConverterConst.DoNothing;
        }
    }
}
