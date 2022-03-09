

namespace SecurityLogin.Cache.Converters
{
    public class NullableCharRedisValueConverter : ICacheValueConverter
    {
        public static readonly NullableCharRedisValueConverter Instance = new NullableCharRedisValueConverter();

        private NullableCharRedisValueConverter() { }

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
