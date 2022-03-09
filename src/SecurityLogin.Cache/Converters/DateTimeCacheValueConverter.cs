
using System;

namespace SecurityLogin.Cache.Converters
{
    public class DateTimeCacheValueConverter : ICacheValueConverter
    {
        public static readonly DateTimeCacheValueConverter Instance = new DateTimeCacheValueConverter();

        private DateTimeCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            var dt = (DateTime)value;
            return dt.Ticks;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            if (value.TryParse(out long tick))
            {
                return new DateTime(tick);
            }
            return CacheValueConverterConst.DoNothing;
        }
    }
}
