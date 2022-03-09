

namespace SecurityLogin.Cache.Converters
{
    public class ByteCacheValueConverter : ICacheValueConverter
    {
        public static readonly ByteCacheValueConverter Instance = new ByteCacheValueConverter();

        private ByteCacheValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(byte)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (byte)(long)value;
        }
    }
}
