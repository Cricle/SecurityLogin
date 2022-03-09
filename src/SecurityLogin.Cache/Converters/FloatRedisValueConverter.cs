

namespace SecurityLogin.Cache.Converters
{
    public class FloatRedisValueConverter : ICacheValueConverter
    {
        public static readonly FloatRedisValueConverter Instance = new FloatRedisValueConverter();

        private FloatRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (float)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (float)value;
        }
    }
}
