using Microsoft.Extensions.DependencyInjection;
using SecurityLogin.Redis.Channel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text;
using System.Linq;
using System;
using SecurityLogin.Redis;
using SecurityLogin.Redis.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using SecurityLogin.Redis.Annotations;

namespace SecurityLogin.RedisCmd
{
    internal class Program
    {
        public class JsonRedisValueConverter : IRedisValueConverter
        {

            public RedisValue Convert(object instance, object value, IRedisColumn column)
            {
                return JsonConvert.SerializeObject(value);
            }

            public object ConvertBack(in RedisValue value, IRedisColumn column)
            {
                if (!value.HasValue)
                {
                    return RedisValueConverterConst.DoNothing;
                }
                return JsonConvert.DeserializeObject(value, column.Property.PropertyType);
            }
        }

        class A
        {
            public int Id { get; set; }

            public long UID { get; set; }

            public B B { get; set; }
        }
        class B
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public C C { get; set; }
        }
        class C
        {
            public int CX { get; set; }

            public string Dx { get; set; }

            public List<int> Lst { get; set; }
        }
        static void Main(string[] args)
        {
            KnowsRedisValueConverter.EndValueConverter = new JsonRedisValueConverter();
            var a = new A { B = new B { Age = 23, Name = "dsadsa" ,C=new C { CX = 44, Dx = "aaa", Lst = new List<int> { 1, 23, 241, 1 } } }, Id = 2, UID = 4213 };
            var val=ExpressionRedisOperator.GetRedisOperator(a.GetType());
            var m=val.As(a);
            var na = new A();
            val.Write(m);
            var d = val.As(a);
            var x = new A();
            object w = x;
            val.Write(ref w,d);
            return;
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