

namespace SecurityLogin.Cache.Converters
{
    public class NullableShortCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableShortCacheValueConverter Instance = new NullableShortCacheValueConverter();

        private NullableShortCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (short?)(char?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (short?)(int?)value;
        }
    }
}
