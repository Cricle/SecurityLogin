using SecurityLogin.Cache.Converters;
using System;
using System.Collections.Generic;

namespace SecurityLogin.Cache
{
    public class RawCacheOperator : EntryCacheOperator
    {
        private static readonly Dictionary<Type, RawCacheOperator> defaultRedisOpCache = new Dictionary<Type, RawCacheOperator>();

        public static RawCacheOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new RawCacheOperator(type);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }

        private ICacheValueConverter converter;

        public RawCacheOperator(Type target) : base(target)
        {
        }

        public override void Build()
        {
            base.Build();
            converter = KnowsRedisValueConverter.GetConverter(Target);
        }

        protected override void WriteCore(ref object instance,in BufferValue entry)
        {
            instance = converter.ConvertBack(entry, null);
        }
        protected override BufferValue AsCore(object value)
        {
            return converter.Convert(null, value, null);
        }
    }
}
