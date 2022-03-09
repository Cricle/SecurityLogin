

namespace SecurityLogin.Cache.Converters
{
    public class NullableShortRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableShortRedisValueConverter Instance = new NullableShortRedisValueConverter();

        private NullableShortRedisValueConverter() { }

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
