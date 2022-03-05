﻿using SecurityLogin.Redis.Converters;
using System;

namespace StackExchange.Redis
{
    public static class RedisConvertExtensions
    {
        private static readonly Type StringType = typeof(string);
        private static readonly Type RedisValueType = typeof(RedisValue);
        private static readonly Type DateTimeType = typeof(DateTime);

        public static T Get<T>(this in RedisValue value)
        {
            return (T)Get(value, typeof(T));
        }
        public static object Get(this in RedisValue value, Type type)
        {
            if (type == RedisValueType)
            {
                return value;
            }
            if (type.IsPrimitive || type == DateTimeType)
            {
                return Convert.ChangeType(value, type);
            }
            if (type == StringType)
            {
                return value.ToString();
            }
            var underType = Nullable.GetUnderlyingType(type);

            if (underType != null)
            {
                try
                {
                    return Get(in value, underType);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            var convert = KnowsRedisValueConverter.GetConverter(type);
            return convert?.ConvertBack(value, null);
        }
    }
}
