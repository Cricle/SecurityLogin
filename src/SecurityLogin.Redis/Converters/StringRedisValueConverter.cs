using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Converters
{
    public class StringRedisValueConverter : IRedisValueConverter
    {
        public static readonly StringRedisValueConverter Instance = new StringRedisValueConverter();
        
        private StringRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (string)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            return value.ToString();
        }
    }
}
