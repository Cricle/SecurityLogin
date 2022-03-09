
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Cache.Converters
{
    internal class EmptyRedisValueConverter: ICacheValueConverter
    {
        public static readonly EmptyRedisValueConverter Instance = new EmptyRedisValueConverter();

        private EmptyRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (BufferValue)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            return value;
        }
    }
}
