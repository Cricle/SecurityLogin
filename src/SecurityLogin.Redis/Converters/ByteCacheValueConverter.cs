﻿using StackExchange.Redis;


namespace SecurityLogin.Cache.Converters
{
    public class ByteCacheValueConverter : ICacheValueConverter
    {
        public static readonly ByteCacheValueConverter Instance = new ByteCacheValueConverter();

        private ByteCacheValueConverter() { }

        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return (long)(byte)value;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            if (!value.HasValue)
            {
                return CacheValueConverterConst.DoNothing;
            }
            return (byte)(long)value;
        }
    }
}