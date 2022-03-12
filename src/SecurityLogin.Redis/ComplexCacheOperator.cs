﻿using Ao.ObjectDesign;
using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace SecurityLogin.Cache
{
    public abstract class ComplexCacheOperator : ICacheOperator 
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

        public abstract void Write(ref object instance, HashEntry[] entries);
        public abstract HashEntry[] As(object value);

        public object Create()
        {
            return creator?.Invoke() ?? null;
        }
    }
}