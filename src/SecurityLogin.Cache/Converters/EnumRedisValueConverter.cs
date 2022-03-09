
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecurityLogin.Cache.Converters
{
    public class EnumRedisValueConverter : ICacheValueConverter
    {
        public static readonly EnumRedisValueConverter Instance = new EnumRedisValueConverter();

        private EnumRedisValueConverter() { }

        public BufferValue Convert(object instance, object value, ICacheColumn column)
        {
            return value.ToString();
        }

        public object ConvertBack(in BufferValue value, ICacheColumn column)
        {
            var val = value.ToString();
            var helper = EnumHelper.GetEnumHelper(column.Property.PropertyType);
            if (helper.TryConvert(val,out var enumVal))
            {
                return enumVal;
            }
            return RedisValueConverterConst.DoNothing;
        }
        class EnumHelper
        {
            private static readonly Dictionary<Type,EnumHelper> enumHelpers = new Dictionary<Type,EnumHelper>();

            public static EnumHelper GetEnumHelper(Type type)
            {
                if (!type.IsEnum)
                {
                    throw new ArgumentException($"Type {type} is not enum");
                }
                if (!enumHelpers.TryGetValue(type,out var helper))
                {
                    helper = new EnumHelper(type);
                    enumHelpers[type]=helper;
                }
                return helper;
            }

            private EnumHelper(Type target)
            {
                map = Enum.GetNames(target).ToDictionary(x => x,
                    x => Enum.Parse(target, x), StringComparer.OrdinalIgnoreCase);
                FirstValue = map.Values.FirstOrDefault();
                if (FirstValue==null)
                {
                    FirstValue = 0;
                }
            }

            private readonly Dictionary<string, object> map;

            public object FirstValue { get; }

            public bool TryConvert(string name,out object value)
            {
                return map.TryGetValue(name, out value);
            }
        }
    }
}
