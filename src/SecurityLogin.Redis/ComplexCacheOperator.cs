using Ao.ObjectDesign;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace SecurityLogin.Redis
{
    public abstract class ComplexCacheOperator
    {
        private IReadOnlyList<ICacheColumn> redisColumns;
        private IReadOnlyDictionary<string, ICacheColumn> redisColumnMap;
        private TypeCreator creator;

        protected static readonly CacheColumnAnalysis SharedAnalysis = new CacheColumnAnalysis();

        protected ComplexCacheOperator(Type target, ICacheColumnAnalysis columnAnalysis)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ColumnAnalysis = columnAnalysis ?? throw new ArgumentNullException(nameof(columnAnalysis));
        }

        public Type Target { get; }

        public ICacheColumnAnalysis ColumnAnalysis { get; }

        public IReadOnlyList<ICacheColumn> RedisColumns => redisColumns;

        public IReadOnlyDictionary<string, ICacheColumn> RedisColumnMap => redisColumnMap;

        public void Build()
        {
            redisColumns = BuildColumns();
            redisColumnMap = BuildColumnMap();
            if (Target.IsPrimitive || Target == typeof(string))
            {
                creator = CompiledPropertyInfo.GetCreator(Target);
            }
            OnBuild();
        }

        protected virtual void OnBuild()
        {

        }

        protected virtual IReadOnlyList<ICacheColumn> BuildColumns()
        {
            return ColumnAnalysis.GetRedisColumns(Target, null);
        }
        protected virtual IReadOnlyDictionary<string, ICacheColumn> BuildColumnMap()
        {
            return ColumnAnalysis.GetRedisColumnMap(Target, null);
        }

        public object Create()
        {
            return creator?.Invoke() ?? null;
        }
    }
}
