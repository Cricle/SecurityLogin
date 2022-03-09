

namespace SecurityLogin.Cache.Converters
{
    public class ShortRedisValueConverter : ICacheValueConverter
    {
        public static readonly ShortRedisValueConverter Instance = new ShortRedisValueConverter();

        private ShortRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(short)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (short)(long)value;
        }
    }
}
