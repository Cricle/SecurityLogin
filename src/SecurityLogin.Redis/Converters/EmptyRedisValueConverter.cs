using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Converters
{
    internal class EmptyRedisValueConverter: IRedisValueConverter
    {
        public static readonly EmptyRedisValueConverter Instance = new EmptyRedisValueConverter();

        private EmptyRedisValueConverter() { }

        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return (RedisValue)value;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
        {
            return value;
        }
    }
}
