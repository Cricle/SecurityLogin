using Ao.ObjectDesign;
using SecurityLogin.Cache;
using SecurityLogin.Cache.Finders;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Finders
{
    internal static class OperatorCacher
    {
        private static readonly Dictionary<Type, ICacheOperator> operatorMap = new Dictionary<Type, ICacheOperator>();

        public static ICacheOperator GetOperator(Type type)
        {
            if (!operatorMap.TryGetValue(type, out var @operator))
            {
                if (type.IsPrimitive ||
                    type == typeof(string) ||
                    Nullable.GetUnderlyingType(type) != null)
                {
                    @operator = RawCacheOperator.GetRedisOperator(type);
                }
                else
                {
                    @operator = ExpressionCacheOperator.GetRedisOperator(type);
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
        private readonly ICacheOperator @operator;
        private readonly ExpressionCacheOperator expressionRedis;
        private readonly TypeCreator Creator;


        protected HashSetCacheFinder(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            if (!IsNormalType)
            {
                Creator = CompiledPropertyInfo.GetCreator(EntityType);
            }
            @operator = OperatorCacher.GetOperator(EntityType);
            expressionRedis = @operator as ExpressionCacheOperator;
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
                    return (TEntity)expressionRedis.Write(data.AsBuffer());
                }
                object inst = Create();
                @operator.WriteRedis(ref inst, data);
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
            var h = @operator.AsRedis(entity);
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