

namespace SecurityLogin.Cache.Converters
{
    public class LongCacheValueConverter : ICacheValueConverter
    {
        public static readonly LongCacheValueConverter Instance = new LongCacheValueConverter();

        private LongCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (value.TryParse(out long val))
            {
                return val;
            }
            return default(long);
        }
    }
}
