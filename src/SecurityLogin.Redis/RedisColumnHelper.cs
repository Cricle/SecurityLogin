using Ao.ObjectDesign;
using SecurityLogin.Redis.Annotations;
using SecurityLogin.Redis.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace SecurityLogin.Redis
{
    public static class RedisColumnHelper
    {
        public static readonly Dictionary<Type, IReadOnlyList<IRedisColumn>> redisColumnMap = new Dictionary<Type, IReadOnlyList<IRedisColumn>>();
        public static readonly Dictionary<Type, IReadOnlyDictionary<string, IRedisColumn>> redisColumnMapMap = new Dictionary<Type, IReadOnlyDictionary<string, IRedisColumn>>();

        public static IReadOnlyDictionary<string, IRedisColumn> GetRedisColumnMap(Type type)
        {
            if (!redisColumnMapMap.TryGetValue(type, out var map))
            {
                var columns = GetRedisColumns(type);
                map = columns.ToDictionary(x => x.Name);
                redisColumnMapMap[type] = map;
            }
            return map;
        }
        public static IReadOnlyList<IRedisColumn> GetRedisColumns(Type type)
        {
            if (type.IsValueType || type == typeof(string))
            {
                throw new ArgumentException($"Type {type} can't parse!");
            }
            if (!redisColumnMap.TryGetValue(type, out var columns))
            {
                columns = Analysis(type);
                redisColumnMap[type] = columns;
            }
            return columns;
        }

        private static IReadOnlyList<IRedisColumn> Analysis(Type type)
        {
            var columns = new List<IRedisColumn>();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<IgnoreColumnAttribute>() == null);
            var convertTypeCache = new Dictionary<Type, IRedisValueConverter>();
            var nameSet = new HashSet<string>();

            foreach (var item in props)
            {
                var converterAttr = item.GetCustomAttribute<RedisValueConverterAttribute>();
                IRedisValueConverter converter = null;
                if (converterAttr != null)
                {
                    if (!convertTypeCache.TryGetValue(converterAttr.ConvertType, out converter))
                    {
                        converter = (IRedisValueConverter)ReflectionHelper.Create(converterAttr.ConvertType);
                        convertTypeCache[converterAttr.ConvertType] = converter;
                    }
                }
                else
                {
                    converter = KnowsRedisValueConverter.GetConverter(item.PropertyType);
                }

                PropertyGetter getter = null;
                PropertySetter setter = null;

                var identity = new PropertyIdentity(item);

                if (item.CanRead)
                {
                    getter = CompiledPropertyInfo.GetGetter(identity);
                }
                if (item.CanWrite)
                {
                    setter = CompiledPropertyInfo.GetSetter(identity);
                }
                var name = item.Name;
                var nameAttr = item.GetCustomAttribute<ColumnAttribute>();
                if (nameAttr != null)
                {
                    name = nameAttr.Name;
                }
                if(!nameSet.Add(name))
                {
                    throw new ArgumentException($"Name {name} in type {type} is not only");
                }
                var column = new RedisColumn
                {
                    Converter = converter,
                    Getter = getter,
                    Setter = setter,
                    Name = name,
                    Property = item,
                    NameRedis = name,
                };
                columns.Add(column);
            }
            return columns.ToArray();
        }
    }
}
