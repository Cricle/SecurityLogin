
using System;

namespace SecurityLogin.Cache.Converters
{
    public class NullableDateTimeCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableDateTimeCacheValueConverter Instance = new NullableDateTimeCacheValueConverter();

        private NullableDateTimeCacheValueConverter() { }

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
                return CacheValueConverterConst.DoNothing;
            }
            if (value.TryParse(out long tick))
            {
                return new DateTime(tick);
            }
            return null;
        }
    }
}
