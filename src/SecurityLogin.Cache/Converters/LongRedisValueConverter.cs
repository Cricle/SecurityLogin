

namespace SecurityLogin.Cache.Converters
{
    public class LongRedisValueConverter : ICacheValueConverter
    {
        public static readonly LongRedisValueConverter Instance = new LongRedisValueConverter();

        private LongRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (value.TryParse(out long val))
            {
                return val;
            }
            return default(long);
        }
    }
}
