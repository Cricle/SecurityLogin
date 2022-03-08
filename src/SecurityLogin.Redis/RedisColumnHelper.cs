using Ao.ObjectDesign;
using SecurityLogin.Redis.Annotations;
using SecurityLogin.Redis.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace SecurityLogin.Redis
{
    public interface IRedisColumnAnalysis
    {
        IReadOnlyDictionary<string, IRedisColumn> GetRedisColumnMap(Type type, string prefx);

        IReadOnlyList<IRedisColumn> GetRedisColumns(Type type, string prefx);
    }
    public class ColumnAnalysis : IRedisColumnAnalysis
    {
        public static readonly Type StringType = typeof(string);
        public static readonly string IListName = typeof(IList).FullName;
        public static readonly string IDictionaryName = typeof(IDictionary).FullName;

        public ColumnAnalysis()
        {
            IgnoreTypes = new HashSet<Type>();
            IgnoreProperties = new HashSet<PropertyInfo>();
            IgnoreNames = new HashSet<string>();
        }

        public HashSet<Type> IgnoreTypes { get; }

        public HashSet<PropertyInfo> IgnoreProperties { get; }

        public HashSet<string> IgnoreNames { get; }

        public bool IgnoreNoSetter { get; set; }

        public IReadOnlyDictionary<string, IRedisColumn> GetRedisColumnMap(Type type, string prefx)
        {
            var columns = GetRedisColumns(type, prefx);
            var map = columns.ToDictionary(x => x.Name);
            return map;
        }

        public IReadOnlyList<IRedisColumn> GetRedisColumns(Type type, string prefx)
        {
            if (type.IsPrimitive || type == typeof(string))
            {
                throw new ArgumentException($"Type {type} can't parse!");
            }
            return Analysis(type, prefx);
        }
        protected virtual bool CanLookup(PropertyInfo info)
        {
            return info.GetCustomAttribute<IgnoreColumnAttribute>() == null &&
                info.GetIndexParameters().Length == 0 &&
                !IgnoreTypes.Contains(info.PropertyType) &&
                !IgnoreProperties.Contains(info) &&
                !IgnoreNames.Contains(info.Name);
        }
        protected virtual bool CanDeep(PropertyInfo info, IRedisColumn column)
        {
            return !info.PropertyType.IsValueType&& info.PropertyType != StringType &&
                info.GetCustomAttribute<NotDeepAttribute>() == null &&
                info.PropertyType.GetInterface(IListName) == null &&
                info.PropertyType.GetInterface(IDictionaryName) == null;
        }
        private IRedisColumn[] Analysis(Type type, string prefx)
        {
            var columns = new List<IRedisColumn>();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(CanLookup);
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
                        var members = converterAttr.ConvertType.GetMember("Instance", BindingFlags.Static| BindingFlags.Public);
                        if (members.Length == 0)
                        {
                            converter = (IRedisValueConverter)ReflectionHelper.Create(converterAttr.ConvertType);
                        }
                        else
                        {
                            var member = members[0];
                            if (member.MemberType== MemberTypes.Field)
                            {
                                converter = (IRedisValueConverter)((FieldInfo)member).GetValue(null);
                            }
                            else if (member.MemberType== MemberTypes.Property)
                            {
                                converter = (IRedisValueConverter)((PropertyInfo)member).GetValue(null);
                            }
                            else
                            {
                                throw new InvalidOperationException("Error to get static member value");
                            }
                        }
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
                else
                {
                    if (IgnoreNoSetter)
                    {
                        continue;
                    }
                }
                var name = item.Name;
                var nameAttr = item.GetCustomAttribute<ColumnAttribute>();
                if (nameAttr != null)
                {
                    name = nameAttr.Name;
                }
                if (!nameSet.Add(name))
                {
                    throw new ArgumentException($"Name {name} in type {type} is not only");
                }
                var column = new RedisColumn
                {
                    Converter = converter,
                    Getter = getter,
                    Setter = setter,
                    Name = name,
                    Path = string.IsNullOrEmpty(prefx) ? name : string.Concat(prefx, ".", name),
                    Property = item,
                    NameRedis = name,
                };
                columns.Add(column);
                if (CanDeep(item,column))
                {
                    var nexts = Analysis(item.PropertyType, name);
                    column.Nexts = nexts;
                }
            }
            return columns.ToArray();
        }

    }
}
