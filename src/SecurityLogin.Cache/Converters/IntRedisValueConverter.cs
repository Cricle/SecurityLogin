

namespace SecurityLogin.Cache.Converters
{
    public class IntRedisValueConverter : ICacheValueConverter
    {
        public static readonly IntRedisValueConverter Instance = new IntRedisValueConverter();

        private IntRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (int)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (value .TryParse(out int val))
            {
                return val;
            }
            return default(int);
        }
    }
}
