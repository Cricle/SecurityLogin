namespace SecurityLogin.Cache.Converters
{
    public class GzipStringCacheValueConverter : ICacheValueConverter
    {
        public static readonly GzipStringCacheValueConverter Instance = new GzipStringCacheValueConverter();

        private GzipStringCacheValueConverter(){ }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var str = (string)value;
            using var buffer = PoolEncoding.GetBytes(str);
            return CompressionHelper.Gzip(buffer.Buffer,0,buffer.Length, attr.Level);
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            return attr.Encoding.GetString(CompressionHelper.UnGzip(value));
        }
    }
}
