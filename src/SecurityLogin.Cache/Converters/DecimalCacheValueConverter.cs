

namespace SecurityLogin.Cache.Converters
{
    public class DecimalCacheValueConverter : ICacheValueConverter
    {
        public static readonly DecimalCacheValueConverter Instance = new DecimalCacheValueConverter();

        private DecimalCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double)((decimal)value);
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (decimal)value;
        }
    }
}
