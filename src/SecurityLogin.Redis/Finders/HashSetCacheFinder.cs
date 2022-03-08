using Ao.ObjectDesign;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Finders
{
    internal static class OperatorCacher
    {
        private static readonly Dictionary<Type, IRedisOperator> operatorMap = new Dictionary<Type, IRedisOperator>();

        public static IRedisOperator GetOperator(Type type)
        {
            if (!operatorMap.TryGetValue(type, out var @operator))
            {
                if (type.IsPrimitive ||
                    type == typeof(string) ||
                    Nullable.GetUnderlyingType(type) != null)
                {
                    @operator = RawRedisOperator.GetRedisOperator(type);
                }
                else
                {
                    @operator = ExpressionRedisOperator.GetRedisOperator(type);
                }
                operatorMap[type] = @operator;
            }
            return @operator;
        }
    }
    public abstract class HashSetCacheFinder<TIdentity, TEntity> : ICacheFinder<TIdentity, TEntity>
    {
        public static readonly TimeSpan DefaultCacheTime = TimeSpan.FromSeconds(3);

        private static readonly bool IsNormalType = typeof(TEntity).IsPrimitive || typeof(TEntity) == typeof(string);
        private static readonly bool IsArray = typeof(TEntity).IsArray;
        private static readonly Type EntityType = typeof(TEntity);
        private readonly IRedisOperator @operator;
        private readonly ExpressionRedisOperator expressionRedis;
        private readonly TypeCreator Creator;


        protected HashSetCacheFinder(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            if (!IsNormalType)
            {
                Creator = CompiledPropertyInfo.GetCreator(EntityType);
            }
            @operator = OperatorCacher.GetOperator(EntityType);
            expressionRedis = @operator as ExpressionRedisOperator;
        }

        public IDatabase Database { get; }

        public async Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            var data = await Database.HashGetAllAsync(key);
            if (data.Length != 0)
            {
                if (expressionRedis != null)
                {
                    return (TEntity)expressionRedis.Write(data);
                }
                object inst = Create();
                @operator.Write(ref inst, data);
                return (TEntity)inst;
            }
            return default;
        }
        protected virtual TEntity Create()
        {
            if (IsNormalType)
            {
                return default;
            }
            if (IsArray)
            {
                throw new InvalidOperationException($"Can't operator raw array!");
            }
            return (TEntity)Creator();
        }
        protected virtual string GetPart(TIdentity identity)
        {
            return identity?.ToString();
        }
        protected virtual string GetHead()
        {
            return GetType().FullName;
        }
        protected string GetEntryKey(TIdentity identity)
        {
            return string.Concat(GetHead(), ".", GetPart(identity));
        }
        public async Task<TEntity> FindInDbAsync(TIdentity identity, bool cache = true)
        {
            var entry = await OnFindInDbAsync(identity);
            if (entry != null && cache)
            {
                await SetInCahceAsync(identity, entry);
            }
            return entry;
        }

        protected abstract Task<TEntity> OnFindInDbAsync(TIdentity identity);

        public async Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity)
        {
            var key = GetEntryKey(identity);
            var h = @operator.As(entity);
            var cacheTime = GetCacheTime(identity, entity);
            await Database.HashSetAsync(key, h);
            return await Database.KeyExpireAsync(key, cacheTime);
        }
        protected virtual TimeSpan? GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DefaultCacheTime;
        }
    }

}