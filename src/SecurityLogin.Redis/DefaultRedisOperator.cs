using Ao.ObjectDesign;
using FastExpressionCompiler;
using SecurityLogin.Redis.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SecurityLogin.Redis
{
    public class DefaultRedisOperator : ComplexRedisOperator
    {
        private static readonly Dictionary<Type, DefaultRedisOperator> defaultRedisOpCache = new Dictionary<Type, DefaultRedisOperator>();

        public static DefaultRedisOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new DefaultRedisOperator(type, SharedAnalysis);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }

        public DefaultRedisOperator(Type target, IRedisColumnAnalysis columnAnalysis)
            :base(target, columnAnalysis)
        {
        }

        public override void Write(ref object instance, HashEntry[] entries)
        {
            WriteAll(ref instance, RedisColumns, entries.ToDictionary(x => x.Name.ToString(), x => x.Value));
        }
        private void WriteAll(ref object instance, IEnumerable<IRedisColumn> columns, IDictionary<string, RedisValue> map)
        {
            foreach (var column in columns)
            {
                if (map.TryGetValue(column.Path, out var val))
                {
                    object value = val;
                    if (column.Converter != null)
                    {
                        value = column.Converter.ConvertBack(val, column);
                    }
                    if (value != RedisValueConverterConst.DoNothing)
                    {
                        column.Setter(instance, value);
                    }
                }
                if (column.Nexts != null && column.Nexts.Count != 0)
                {
                    var next = CreateInstance(column.Property.PropertyType);
                    WriteAll(ref next, column.Nexts, map);
                    column.Setter(instance, next);
                }
            }
        }

        protected virtual object CreateInstance(Type type)
        {
            return ReflectionHelper.Create(type);
        }

        public override HashEntry[] As(object instance)
        {
            return GetHashEntries(instance, RedisColumns).ToArray();
        }

        private IEnumerable<HashEntry> GetHashEntries(object instance, IEnumerable<IRedisColumn> columns)
        {
            foreach (var column in columns)
            {
                var val = column.Getter(instance);
                if (column.Nexts != null && column.Nexts.Count != 0)
                {
                    foreach (var item in GetHashEntries(val, column.Nexts))
                    {
                        yield return item;
                    }
                    continue;
                }
                var redisVal = column.Converter?.Convert(instance, val, column);
                if (redisVal == null || redisVal.Value.IsNull)
                {
                    redisVal = RedisValue.EmptyString;
                }
                yield return new HashEntry(column.Path, redisVal.Value);
            }
        }
    }
}
