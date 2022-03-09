

namespace SecurityLogin.Cache.Converters
{
    public class NullableFloatRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableFloatRedisValueConverter Instance = new NullableFloatRedisValueConverter();

        private NullableFloatRedisValueConverter() { }

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
