
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Cache.Converters
{
    public static class KnowsCacheValueConverter
    {
        public static ICacheValueConverter EndValueConverter { get; set; }

        public static ICacheValueConverter GetConverter(Type type)
        {
            if (type.IsEquivalentTo(typeof(string)))
            {
                return StringCacheValueConverter.Instance;
            }
            if (type.IsEquivalentTo(typeof(BufferValue)))
            {
                return EmptyCacheValueConverter.Instance;
            }
            if (type.IsValueType)
            {
                if (type.IsEquivalentTo(typeof(char)))
                {
                    return CharCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(char?)))
                {
                    return NullableCharCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(short)))
                {
                    return ShortCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(short?)))
                {
                    return NullableShortCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(uint)))
                {
                    return UIntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(uint?)))
                {
                    return NullableUIntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(ulong)))
                {
                    return ULongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(ulong?)))
                {
                    return NullableULongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(int)))
                {
                    return IntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(int?)))
                {
                    return NullableIntCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(long)))
                {
                    return LongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(long?)))
                {
                    return NullableLongCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(double)))
                {
                    return DoubleCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(double?)))
                {
                    return NullableDoubleCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(float)))
                {
                    return FloatCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(float?)))
                {
                    return NullableFloatCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(decimal)))
                {
                    return DecimalCacheValueConverter.Instance;
                }
                if (type.IsEquivalentTo(typeof(decimal?)))
                {
                    return NullableDecimalCacheValueConverter.Instance;
                }                
            }
            if (type.IsEquivalentTo(typeof(byte[])))
            {
                return ByteArrayCacheValueConverter.Instance;
            }
            if (type.IsEquivalentTo(typeof(DateTime)))
            {
                return DateTimeCacheValueConverter.Instance;
            }
            if (type.IsEquivalentTo(typeof(DateTime?)))
            {
                return NullableDateTimeCacheValueConverter.Instance;
            }
            if (type.IsEnum)
            {
                return EnumCacheValueConverter.Instance;
            }

            return EndValueConverter;
        }
    }
}
