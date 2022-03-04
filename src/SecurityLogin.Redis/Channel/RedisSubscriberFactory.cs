using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Channel
{
    public class RedisSubscriberFactory : IRedisSubscriberFactory, IDisposable
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IConnectionMultiplexer connection;
        private readonly List<(RedisChannel, Action<RedisChannel, RedisValue>)> subscribers =
            new List<(RedisChannel, Action<RedisChannel, RedisValue>)>();

        public RedisSubscriberFactory(IServiceScopeFactory serviceScopeFactory,
            IConnectionMultiplexer connection)
        {
            this.connection = connection;
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public Action<RedisChannel, RedisValue> RegistSubscriber(RedisChannel channel, IRedisSubscriber subscriber)
        {
            var val = CreateSubscriber(subscriber);
            subscribers.Add((channel, val));
            connection.GetSubscriber().Subscribe(channel, val);
            return val;
        }
        public Action<RedisChannel, RedisValue> RegistFromServiceSubscriber(RedisChannel channel, Type redisSubscripberType)
        {
            var val = CreateFromServiceSubscriber(redisSubscripberType);
            subscribers.Add((channel, val));
            connection.GetSubscriber().Subscribe(channel, val);
            return val;
        }
        public Action<RedisChannel, RedisValue> RegistFromServiceOnceSubscriber(RedisChannel channel, Type redisSubscripberType, TimeSpan? lockTime = null, LogLevel logLevel = LogLevel.Trace)
        {
            var val = CreateFromServiceOnceSubscriber(redisSubscripberType, lockTime, logLevel);
            subscribers.Add((channel, val));
            connection.GetSubscriber().Subscribe(channel, val);
            return val;
        }
        public Action<RedisChannel, RedisValue> CreateFromServiceOnceSubscriber(Type redisSubscriberType, TimeSpan? lockTime = null, LogLevel logLevel = LogLevel.Trace)
        {
            CheckSubscribeType(redisSubscriberType);
            var typeName = redisSubscriberType.FullName;
            var val = new Action<RedisChannel, RedisValue>(async (c, v) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<RedisSubscriberFactory>>();
                var sub = scope.ServiceProvider.GetRequiredService(redisSubscriberType) as IRedisSubscriber;
                try
                {
                    await sub.DoAsync(c, v, scope.ServiceProvider);
                    logger.Log(logLevel, "Complated execute {0} at type {1}", c, typeName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "In channel {0}, when call {1}", c, sub.GetType().FullName);
                    throw;
                }
            });
            return val;
        }
        private Action<RedisChannel, RedisValue> CheckSubscribeType(Type redisSubscriberType)
        {
            if (redisSubscriberType is null)
            {
                throw new ArgumentNullException(nameof(redisSubscriberType));
            }
            if (redisSubscriberType.GetInterface(typeof(IRedisSubscriber).FullName) == null)
            {
                throw new ArgumentException($"Type {redisSubscriberType.FullName} is not implement {typeof(IRedisSubscriber).FullName}!");
            }
            var val = new Action<RedisChannel, RedisValue>(async (c, v) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var sub = scope.ServiceProvider.GetRequiredService(redisSubscriberType) as IRedisSubscriber;
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<RedisSubscriberFactory>>();
                try
                {
                    await sub.DoAsync(c, v, scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "In channel {0}, when call {1}", c, sub.GetType().FullName);
                    throw;
                }
            });
            return val;
        }
        public Action<RedisChannel, RedisValue> CreateFromServiceSubscriber(Type redisSubscriberType)
        {
            CheckSubscribeType(redisSubscriberType);
            var val = new Action<RedisChannel, RedisValue>(async (c, v) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var sub = scope.ServiceProvider.GetRequiredService(redisSubscriberType) as IRedisSubscriber;
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<RedisSubscriberFactory>>();
                try
                {
                    await sub.DoAsync(c, v, scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "In channel {0}, when call {1}", c, sub.GetType().FullName);
                    throw;
                }
            });
            return val;
        }
        public Action<RedisChannel, RedisValue> CreateSubscriber(IRedisSubscriber subscriber)
        {
            if (subscriber is null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }

            var val = new Action<RedisChannel, RedisValue>(async (c, v) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<RedisSubscriberFactory>>();
                try
                {
                    await subscriber.DoAsync(c, v, scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    throw;
                }
            });
            return val;
        }

        public void Dispose()
        {
            var sub = connection.GetSubscriber();
            foreach (var item in subscribers)
            {
                sub.Unsubscribe(item.Item1, item.Item2);
            }
            subscribers.Clear();
        }
    }
}
