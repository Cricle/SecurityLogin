using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace SecurityLogin.Redis
{
    public abstract class ComplexRedisOperator : IRedisOperator 
    {
        private IReadOnlyList<IRedisColumn> redisColumns;
        private IReadOnlyDictionary<string, IRedisColumn> redisColumnMap;

        protected static readonly ColumnAnalysis SharedAnalysis = new ColumnAnalysis();

        protected ComplexRedisOperator(Type target, IRedisColumnAnalysis columnAnalysis)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            ColumnAnalysis = columnAnalysis ?? throw new ArgumentNullException(nameof(columnAnalysis));
        }

        public Type Target { get; }

        public IRedisColumnAnalysis ColumnAnalysis { get; }

        public IReadOnlyList<IRedisColumn> RedisColumns => redisColumns;

        public IReadOnlyDictionary<string, IRedisColumn> RedisColumnMap => redisColumnMap;

        public void Build()
        {
            redisColumns = BuildColumns();
            redisColumnMap = BuildColumnMap();
            OnBuild();
        }

        protected virtual void OnBuild()
        {

        }

        protected virtual IReadOnlyList<IRedisColumn> BuildColumns()
        {
            return ColumnAnalysis.GetRedisColumns(Target, null);
        }
        protected virtual IReadOnlyDictionary<string, IRedisColumn> BuildColumnMap()
        {
            return ColumnAnalysis.GetRedisColumnMap(Target, null);
        }

        public abstract void Write(ref object instance, HashEntry[] entries);
        public abstract HashEntry[] As(object value);
    }
}
