

namespace SecurityLogin.Cache.Converters
{
    public class ShortCacheValueConverter : ICacheValueConverter
    {
        public static readonly ShortCacheValueConverter Instance = new ShortCacheValueConverter();

        private ShortCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(short)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (short)(long)value;
        }
    }
}
