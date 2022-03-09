

namespace SecurityLogin.Cache.Converters
{
    public class ULongCacheValueConverter : ICacheValueConverter
    {
        public static readonly ULongCacheValueConverter Instance = new ULongCacheValueConverter();

        private ULongCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (ulong)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (ulong)value;
        }
    }
}
