using Microsoft.Extensions.DependencyInjection;
using SecurityLogin.Redis.Channel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text;
using System.Linq;
using System;

namespace SecurityLogin.RedisCmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run().GetAwaiter().GetResult();
        }
        private static async Task Run()
        {
            var conn = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            var provider = new ServiceCollection()
                .AddLogging(x => x.AddConsole())
                .AddScoped<Reader>()
                .AddSingleton(conn)
                .BuildServiceProvider();
            var ser = new RedisSubscriberFactory(provider.GetRequiredService<IServiceScopeFactory>(), conn);
            ser.RegistFromServiceSubscriber("hello", typeof(Reader));
            var db = conn.GetDatabase();
            for (int i = 0; i < 10; i++)
            {
                db.Publish("hello", "www" + i);
                await Task.Delay(500);
            }
        }
    }
    class Reader : IRedisSubscriber
    {
        private readonly ILogger<Reader> logger;
        private readonly ConnectionMultiplexer connectionMultiplexer;

        public Reader(ILogger<Reader> logger,
            ConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.logger = logger;
        }



        public Task DoAsync(RedisChannel channel, RedisValue value, IServiceProvider serviceProvider)
        {
            logger.LogInformation(value);
            return Task.CompletedTask;
        }
    }
}