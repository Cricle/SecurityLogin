using Microsoft.IO;
using SecurityLogin.Redis.Annotations;
using StackExchange.Redis;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Converters
{
    public class GzipStringRedisValueConverter : IRedisValueConverter
    {
        public static readonly GzipStringRedisValueConverter Instance = new GzipStringRedisValueConverter();

        private GzipStringRedisValueConverter(){ }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var str = (string)value;
            using var buffer = PoolEncoding.GetBytes(str);
            return CompressionHelper.Gzip(buffer.Buffer,0,buffer.Length, attr.Level);
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            return attr.Encoding.GetString(CompressionHelper.UnGzip(value));
        }
    }
    public class DeflateStringRedisValueConverter : IRedisValueConverter
    {
        public static readonly DeflateStringRedisValueConverter Instance = new DeflateStringRedisValueConverter();

        private DeflateStringRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var str = (string)value;
            using var buffer = PoolEncoding.GetBytes(str);
            return CompressionHelper.Deflate(buffer.Buffer, 0, buffer.Length, attr.Level);
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            return attr.Encoding.GetString(CompressionHelper.UnDeflate(value));
        }
    }
}
