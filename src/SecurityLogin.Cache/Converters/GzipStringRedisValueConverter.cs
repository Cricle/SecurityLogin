using Microsoft.IO;
using SecurityLogin.Cache.Annotations;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Cache.Converters
{
    public class MessagePackStringRedisValueConverter : ICacheValueConverter
    {
        public static readonly MessagePackStringRedisValueConverter Instance = new MessagePackStringRedisValueConverter();

        private MessagePackStringRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var str = (string)value;
            using var buffer = PoolEncoding.GetBytes(str);
            return CompressionHelper.Gzip(buffer.Buffer, 0, buffer.Length, attr.Level);
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            return attr.Encoding.GetString(CompressionHelper.UnGzip(value));
        }
    }
    public class GzipStringRedisValueConverter : ICacheValueConverter
    {
        public static readonly GzipStringRedisValueConverter Instance = new GzipStringRedisValueConverter();

        private GzipStringRedisValueConverter(){ }

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
    public class DeflateStringRedisValueConverter : ICacheValueConverter
    {
        public static readonly DeflateStringRedisValueConverter Instance = new DeflateStringRedisValueConverter();

        private DeflateStringRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            var str = (string)value;
            using var buffer = PoolEncoding.GetBytes(str);
            return CompressionHelper.Deflate(buffer.Buffer, 0, buffer.Length, attr.Level);
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            var attr = CompressionHelper.GetAttribute(column);
            return attr.Encoding.GetString(CompressionHelper.UnDeflate(value));
        }
    }
}
