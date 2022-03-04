using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Converters
{
    public static class KnowsRedisValueConverter
    {
        public static IRedisValueConverter EndValueConverter { get; set; }

        public static IRedisValueConverter GetConverter(Type type)
        {
            if (type.IsEquivalentTo(typeof(string)))
            {
                return StringRedisValueConverter.Instance;
            }
            if (type.IsEquivalentTo(typeof(RedisValue)))
            {
                return EmptyRedisValueConverter.Instance;
            }
            if (type.IsValueType)
            {
                if (type.IsEquivalentTo(typeof(char)))
                {
                    return CharRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(char?)))
                {
                    return NullableCharRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(short)))
                {
                    return ShortRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(short?)))
                {
                    return NullableShortRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(uint)))
                {
                    return UIntRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(uint?)))
                {
                    return NullableUIntRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(ulong)))
                {
                    return ULongRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(ulong?)))
                {
                    return NullableULongRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(int)))
                {
                    return IntRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(int?)))
                {
                    return NullableIntRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(long)))
                {
                    return LongRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(long?)))
                {
                    return NullableLongRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(double)))
                {
                    return DoubleRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(double?)))
                {
                    return NullableDoubleRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(float)))
                {
                    return FloatRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(float?)))
                {
                    return NullableFloatRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(decimal)))
                {
                    return DecimalRedisValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(decimal?)))
                {
                    return NullableDecimalRedisValueConverter.Instance;
                }                
            }
            if (type.IsEquivalentTo(typeof(byte[])))
            {
                return ByteArrayRedisValueConverter.Instance;
            }
            if (type.IsEquivalentTo(typeof(DateTime)))
            {
                return DateTimeRedisValueConverter.Instance;
            }
            if (type.IsEquivalentTo(typeof(DateTime?)))
            {
                return NullableDateTimeRedisValueConverter.Instance;
            }
            if (type.IsEnum)
            {
                return EnumRedisValueConverter.Instance;
            }

            return EndValueConverter;
        }
    }
}
