using SecurityLogin.Redis.Annotations;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Converters
{
    public class GzipRedisValueConverter : IRedisValueConverter
    {
        public static readonly GzipRedisValueConverter Instance = new GzipRedisValueConverter();


        private GzipRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var buffer=(byte[])value;
            return CompressionHelper.Gzip(buffer, attr.Level);
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
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
