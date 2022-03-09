

namespace SecurityLogin.Cache.Converters
{
    public class NullableUIntRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableUIntRedisValueConverter Instance = new NullableUIntRedisValueConverter();

        private NullableUIntRedisValueConverter() { }

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
