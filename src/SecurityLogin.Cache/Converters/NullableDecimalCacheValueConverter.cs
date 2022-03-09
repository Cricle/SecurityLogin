

namespace SecurityLogin.Cache.Converters
{
    public class NullableDecimalCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableDecimalCacheValueConverter Instance = new NullableDecimalCacheValueConverter();

        private NullableDecimalCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double?)((decimal?)value);
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (decimal?)value;
        }
    }
}
