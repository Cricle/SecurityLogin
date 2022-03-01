using SecurityLogin.Redis.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace SecurityLogin.Redis
{
    public class DefaultRedisOperator : IRedisOperator
    {
        private static readonly Dictionary<Type, DefaultRedisOperator> defaultRedisOpCache = new Dictionary<Type, DefaultRedisOperator>();

        public static DefaultRedisOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new DefaultRedisOperator(type);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }

        private IReadOnlyList<IRedisColumn> redisColumns;
        private IReadOnlyDictionary<string, IRedisColumn> redisColumnMap;

        public DefaultRedisOperator(Type target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public Type Target { get; }

        public IReadOnlyList<IRedisColumn> RedisColumns => redisColumns;

        public IReadOnlyDictionary<string, IRedisColumn> RedisColumnMap => redisColumnMap;

        public void Build()
        {
            redisColumns = BuildColumns();
            redisColumnMap = BuildColumnMap();
        }
        protected virtual IReadOnlyList<IRedisColumn> BuildColumns()
        {
            return RedisColumnHelper.GetRedisColumns(Target);
        }
        protected virtual IReadOnlyDictionary<string, IRedisColumn> BuildColumnMap()
        {
            return RedisColumnHelper.GetRedisColumnMap(Target);
        }

        public void Write(ref object instance, HashEntry[] entries)
        {
            var count = entries.Length;
            for (int i = 0; i < count; i++)
            {
                var item = entries[i];
                if (redisColumnMap.TryGetValue(item.Name, out var column) && column.Setter != null)
                {
                    object val = item.Value;
                    if (column.Converter != null)
                    {
                        val = column.Converter.ConvertBack(item.Value, column);
                    }
                    if (val != RedisValueConverterConst.DoNothing)
                    {
                        column.Setter(instance, val);
                    }
                }
            }
        }

        public HashEntry[] As(object instance)
        {
            var count = redisColumns.Count;
            var entries = new HashEntry[count];
            for (int i = 0; i < count; i++)
            {
                var item = redisColumns[i];
                var val = item.Getter(instance);
                var redisVal = item.Converter.Convert(instance, val, item);
                if (redisVal.IsNull)
                {
                    redisVal = RedisValue.EmptyString;
                }
                entries[i] = new HashEntry(item.NameRedis, redisVal);
            }
            return entries;
        }
    }
}
