

namespace SecurityLogin.Cache.Converters
{
    public class NullableIntCacheValueConverter : ICacheValueConverter
    {
        public static readonly NullableIntCacheValueConverter Instance = new NullableIntCacheValueConverter();

        private NullableIntCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (int?)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return null; 
            }
            if (value.TryParse(out int val))
            {
                return val;
            }
            return null;
        }
    }
}
