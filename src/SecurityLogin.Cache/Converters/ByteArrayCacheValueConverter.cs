
using System;

namespace SecurityLogin.Cache.Converters
{

    public class ByteArrayCacheValueConverter : ICacheValueConverter
    {
        public static readonly ByteArrayCacheValueConverter Instance = new ByteArrayCacheValueConverter();

        private ByteArrayCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (byte[])value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (byte[])value;
        }
    }
}
