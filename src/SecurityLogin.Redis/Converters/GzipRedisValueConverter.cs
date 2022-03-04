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

        private static readonly GzipSettingAttribute defaultGzipSetting = new GzipSettingAttribute();

        private GzipRedisValueConverter() { }

        private GzipSettingAttribute GetAttribute(IRedisColumn column)
        {
            return column.Property.GetCustomAttribute<GzipSettingAttribute>() ?? defaultGzipSetting;
        }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            var attr = GetAttribute(column);
            byte[] buffer;
            if (value is string str)
            {
                buffer = attr.Encoding.GetBytes(str);
            }
            else if (value is byte[] bytes)
            {
                buffer = bytes;
            }
            else
            {
                throw new ArgumentException($"{value?.GetType()} is not string or byte[]");
            }
            using (var s1 = SharedMemoryStream.StreamManager.GetStream())
            using (var gs = new GZipStream(s1, attr.Level))
            {
                gs.Write(buffer, 0, buffer.Length);
                gs.Flush();
                return s1.ToArray();
            }
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            var buffer= (byte[])value;
            using (var s = SharedMemoryStream.StreamManager.GetStream(buffer))
            using (var s1 = SharedMemoryStream.StreamManager.GetStream())
            using (var gs = new GZipStream(s,CompressionMode.Decompress))
            {
                gs.CopyTo(s1);
                return s1.ToArray();
            }
        }
    }
}
