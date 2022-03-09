

namespace SecurityLogin.Cache.Converters
{
    public class ByteRedisValueConverter : ICacheValueConverter
    {
        public static readonly ByteRedisValueConverter Instance = new ByteRedisValueConverter();

        private ByteRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(byte)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (byte)(long)value;
        }
    }
}
