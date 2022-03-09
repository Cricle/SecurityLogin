

namespace SecurityLogin.Cache.Converters
{
    public class NullableDoubleCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableDoubleCacheValueConverter Instance = new NullableDoubleCacheValueConverter();

        private NullableDoubleCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            if (value.TryParse(out double val))
            {
                return val;
            }
            return null;
        }
    }
}
