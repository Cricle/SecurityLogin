

namespace SecurityLogin.Cache.Converters
{
    public class ULongRedisValueConverter : ICacheValueConverter
    {
        public static readonly ULongRedisValueConverter Instance = new ULongRedisValueConverter();

        private ULongRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (ulong)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (ulong)value;
        }
    }
}
