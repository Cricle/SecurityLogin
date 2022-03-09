
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Cache.Converters
{
    public class StringRedisValueConverter : ICacheValueConverter
    {
        public static readonly StringRedisValueConverter Instance = new StringRedisValueConverter();
        
        private StringRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (string)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            return value.ToString();
        }
    }
}
