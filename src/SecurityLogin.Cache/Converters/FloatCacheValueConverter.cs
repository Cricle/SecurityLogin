

namespace SecurityLogin.Cache.Converters
{
    public class FloatCacheValueConverter : ICacheValueConverter
    {
        public static readonly FloatCacheValueConverter Instance = new FloatCacheValueConverter();

        private FloatCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (float)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (float)value;
        }
    }
}
