

namespace SecurityLogin.Cache.Converters
{
    public class NullableLongRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableLongRedisValueConverter Instance = new NullableLongRedisValueConverter();

        private NullableLongRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            if (value.TryParse(out long val))
            {
                return val;
            }
            return null;
        }
    }
}
