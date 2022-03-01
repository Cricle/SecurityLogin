using Microsoft.IO;
using StackExchange.Redis;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Converters
{
    public class GzipStringRedisValueConverter : IRedisValueConverter
    {
        public static readonly GzipStringRedisValueConverter Shared = new GzipStringRedisValueConverter(CompressionLevel.Fastest);

        public GzipStringRedisValueConverter(CompressionLevel level)
        {
            Level = level;
        }

        public CompressionLevel Level { get; }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            var str = (string)value;
            using var buffer = PoolEncoding.GetBytes(str);
            using var stream = SharedMemoryStream.StreamManager.GetStream(string.Empty, buffer.Buffer, 0, buffer.Length);
            var gzipStream = new GZipStream(stream, Level);
            using var destStream = SharedMemoryStream.StreamManager.GetStream();
            gzipStream.CopyTo(destStream);
            destStream.Seek(0, SeekOrigin.Begin);
            var arr = destStream.ToArray();
            return arr;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            byte[] buffer = value;
            using var stream = SharedMemoryStream.StreamManager.GetStream(string.Empty, buffer, 0, buffer.Length);
            using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(gzipStream);
            return streamReader.ReadToEnd();
        }
    }
}
