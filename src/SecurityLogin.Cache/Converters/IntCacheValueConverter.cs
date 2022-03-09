

namespace SecurityLogin.Cache.Converters
{
    public class IntCacheValueConverter : ICacheValueConverter
    {
        public static readonly IntCacheValueConverter Instance = new IntCacheValueConverter();

        private IntCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (int)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (value .TryParse(out int val))
            {
                return val;
            }
            return default(int);
        }
    }
}
