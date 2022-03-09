

namespace SecurityLogin.Cache.Converters
{
    public class NullableUIntCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableUIntCacheValueConverter Instance = new NullableUIntCacheValueConverter();

        private NullableUIntCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (uint?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (uint?)value;
        }
    }
}
