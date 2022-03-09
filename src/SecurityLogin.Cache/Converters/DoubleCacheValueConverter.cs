

namespace SecurityLogin.Cache.Converters
{
    public class DoubleCacheValueConverter : ICacheValueConverter
    {
        public static readonly DoubleCacheValueConverter Instance = new DoubleCacheValueConverter();

        private DoubleCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (double)value;
        }
    }
}
