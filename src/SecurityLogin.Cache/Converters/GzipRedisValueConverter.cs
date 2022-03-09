using SecurityLogin.Cache.Annotations;

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Cache.Converters
{
    public class GzipRedisValueConverter : ICacheValueConverter
    {
        public static readonly GzipRedisValueConverter Instance = new GzipRedisValueConverter();


        private GzipRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var buffer=(byte[])value;
            return CompressionHelper.Gzip(buffer, attr.Level);
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            var buffer= (byte[])value;
            return CompressionHelper.UnGzip(buffer);
        }
    }
}
