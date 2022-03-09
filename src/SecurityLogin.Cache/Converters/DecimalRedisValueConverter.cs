

namespace SecurityLogin.Cache.Converters
{
    public class DecimalRedisValueConverter : ICacheValueConverter
    {
        public static readonly DecimalRedisValueConverter Instance = new DecimalRedisValueConverter();

        private DecimalRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (double)((decimal)value);
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (decimal)value;
        }
    }
}
