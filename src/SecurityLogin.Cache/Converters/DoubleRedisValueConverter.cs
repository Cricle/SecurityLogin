

namespace SecurityLogin.Cache.Converters
{
    public class DoubleRedisValueConverter : ICacheValueConverter
    {
        public static readonly DoubleRedisValueConverter Instance = new DoubleRedisValueConverter();

        private DoubleRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (double)value;
        }
    }

    public class CharRedisValueConverter : ICacheValueConverter
    {
        public static readonly CharRedisValueConverter Instance = new CharRedisValueConverter();

        private CharRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(char)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (char)(long)value;
        }
    }
}
