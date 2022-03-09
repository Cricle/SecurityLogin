

namespace SecurityLogin.Cache.Converters
{
    public class UIntRedisValueConverter : ICacheValueConverter
    {
        public static readonly UIntRedisValueConverter Instance = new UIntRedisValueConverter();

        private UIntRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return (uint)value;
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return RedisValueConverterConst.DoNothing;
            }
            return (uint)value;
        }
    }
}
