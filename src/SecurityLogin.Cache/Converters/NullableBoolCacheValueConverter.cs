

namespace SecurityLogin.Cache.Converters
{
    public class NullableBoolCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableBoolCacheValueConverter Instance = new NullableBoolCacheValueConverter();

        private NullableBoolCacheValueConverter() { }

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
