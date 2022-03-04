using Ao.ObjectDesign;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Finders
{
    public abstract class HashSetCacheFinder<TIdentity, TEntity> : ICacheFinder<TIdentity, TEntity>
    {
        public static readonly TimeSpan DefaultCacheTime = TimeSpan.FromSeconds(3);

        private static readonly bool IsNormalType = typeof(TEntity).IsPrimitive || typeof(TEntity) == typeof(string);
        private static readonly bool IsArray = typeof(TEntity).IsArray;
        private static readonly IRedisOperator @operator;
        private static readonly Type EntityType = typeof(TEntity);
        private static readonly TypeCreator Creator;

        static HashSetCacheFinder()
        {
            if (!IsNormalType&&!IsArray)
            {
                @operator= DefaultRedisOperator.GetRedisOperator(EntityType);
                Creator = CompiledPropertyInfo.GetCreator(EntityType);
            }
            else
            {
                @operator= RawRedisOperator.GetRedisOperator(EntityType);
            }

        }

        protected HashSetCacheFinder(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public IDatabase Database { get; }

        public async Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            var data = await Database.HashGetAllAsync(key);
            if (data.Length != 0)
            {
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
            var cacheTime= GetCacheTime(identity, entity);
            await Database.HashSetAsync(key, h);
            await Database.KeyExpireAsync(key, cacheTime);
            return true;
        }
        protected virtual TimeSpan GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DefaultCacheTime;
        }
    }
}
