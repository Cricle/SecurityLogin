

namespace SecurityLogin.Cache.Converters
{
    public class NullableULongCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableULongCacheValueConverter Instance = new NullableULongCacheValueConverter();

        private NullableULongCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (ulong?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (ulong?)value;
        }
    }
}
