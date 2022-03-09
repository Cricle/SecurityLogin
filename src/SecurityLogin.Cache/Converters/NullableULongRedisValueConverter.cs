

namespace SecurityLogin.Cache.Converters
{
    public class NullableULongRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableULongRedisValueConverter Instance = new NullableULongRedisValueConverter();

        private NullableULongRedisValueConverter() { }

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
