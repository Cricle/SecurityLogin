
using System;

namespace SecurityLogin.Cache.Converters
{
    public class NullableDateTimeRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableDateTimeRedisValueConverter Instance = new NullableDateTimeRedisValueConverter();

        private NullableDateTimeRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            var dt = (DateTime?)value;
            if (dt == null)
            {
                return BufferValue.EmptyString;
            }
            return dt.Value.Ticks;
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
            return null;
        }
    }
}
