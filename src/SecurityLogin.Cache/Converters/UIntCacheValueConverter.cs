

namespace SecurityLogin.Cache.Converters
{
    public class UIntCacheValueConverter : ICacheValueConverter
    {
        public static readonly UIntCacheValueConverter Instance = new UIntCacheValueConverter();

        private UIntCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (uint)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (uint)value;
        }
    }
}
