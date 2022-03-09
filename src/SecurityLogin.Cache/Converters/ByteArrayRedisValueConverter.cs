
using System;

namespace SecurityLogin.Cache.Converters
{

    public class ByteArrayRedisValueConverter : ICacheValueConverter
    {
        public static readonly ByteArrayRedisValueConverter Instance = new ByteArrayRedisValueConverter();

        private ByteArrayRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (byte[])value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (byte[])value;
        }
    }
}
