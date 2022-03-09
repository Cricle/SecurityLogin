

namespace SecurityLogin.Cache.Converters
{
    public class NullableDecimalRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableDecimalRedisValueConverter Instance = new NullableDecimalRedisValueConverter();

        private NullableDecimalRedisValueConverter() { }

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
