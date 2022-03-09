

namespace SecurityLogin.Cache.Converters
{
    public class NullableCharCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableCharCacheValueConverter Instance = new NullableCharCacheValueConverter();

        private NullableCharCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (int?)(char?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (char?)(int?)value;
        }
    }
}
