using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Channel
{
    public interface IRedisSubscriber
    {
        Task DoAsync(RedisChannel channel, RedisValue value, IServiceProvider serviceProvider);
    }
}