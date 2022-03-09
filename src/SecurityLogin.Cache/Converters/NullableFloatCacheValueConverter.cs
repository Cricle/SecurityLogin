

namespace SecurityLogin.Cache.Converters
{
    public class NullableFloatCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableFloatCacheValueConverter Instance = new NullableFloatCacheValueConverter();

        private NullableFloatCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (float?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (float?)value;
        }
    }
}
