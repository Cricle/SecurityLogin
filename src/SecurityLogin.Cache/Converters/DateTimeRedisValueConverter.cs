
using System;

namespace SecurityLogin.Cache.Converters
{
    public class DateTimeRedisValueConverter : ICacheValueConverter
    {
        public static readonly DateTimeRedisValueConverter Instance = new DateTimeRedisValueConverter();

        private DateTimeRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            var dt = (DateTime)value;
            return dt.Ticks;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
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
