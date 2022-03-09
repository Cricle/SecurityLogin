

namespace SecurityLogin.Cache.Converters
{
    public class NullableBoolRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableBoolRedisValueConverter Instance = new NullableBoolRedisValueConverter();

        private NullableBoolRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (bool?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (bool?)value;
        }
    }
}
