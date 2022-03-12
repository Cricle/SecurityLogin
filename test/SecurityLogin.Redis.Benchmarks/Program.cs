using BenchmarkDotNet.Attributes;
using MessagePack;
using MessagePack.Resolvers;
using SecurityLogin.Cache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SecurityLogin.Redis.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //new ToEntitiesComparer().Setup();
            //var m = new MPCacheOperator(typeof(A));
            //var e = m.As(new A());
            //m.Write(e);
            BenchmarkDotNet.Running.BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
        }
    }
    public class MPCacheOperator : EntryCacheOperator
    {
        private static readonly MessagePackSerializerOptions opt = MessagePackSerializerOptions.Standard.WithResolver(TypelessObjectResolver.Instance)
            .WithCompression( MessagePackCompression.Lz4Block);

        public MPCacheOperator(Type type)
            :base(type)
        {
        }

        protected override RedisValue AsCore(object value)
        {
            return MessagePackSerializer.Serialize(Target, value, opt);
        }

        protected override void WriteCore(ref object instance,in RedisValue entry)
        {
            instance = MessagePackSerializer.Deserialize(Target, (byte[])entry, opt);
        }
        protected override object CreateInstance()
        {
            return null;
        }
    }
    public class A
    {
        public int Id { get; set; }

        public long UID { get; set; }

        public B B { get; set; }
    }
    public class B
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public C C { get; set; }
    }
    public class C
    {
        public int CX { get; set; }

        public string Dx { get; set; }
    }
    public abstract class ConvertRedisBase
    {
        public DefaultCacheOperator @operator;
        public ExpressionCacheOperator @exoperator;
        public MPCacheOperator @mpoperator;
        public HashEntry[] hashEntries;
        public HashEntry[] hhashEntries;
        public RedisValue mphash;
        public A a;
        [GlobalSetup]
        public void Setup()
        {
            a = new A { B = new B { Age = 23, Name = "dsadsa", C = new C { CX = 44, Dx = "aaa" } }, Id = 2, UID = 4213 };
            @operator = DefaultCacheOperator.GetRedisOperator(a.GetType());
            @exoperator= ExpressionCacheOperator.GetRedisOperator(a.GetType());
            @mpoperator = new MPCacheOperator(a.GetType());
            hashEntries = @operator.As(a);
            hhashEntries = @exoperator.As(a);
            @exoperator.As(a);
            mphash =@mpoperator.As(a);
            var x = new A();
            object w = x;
            @operator.Write(ref w, hashEntries);
            @exoperator.Write(ref w, hashEntries);
            @mpoperator.Write(mphash);
            OnSetup();
        }

        protected virtual void OnSetup()
        {

        }
    }
    [MemoryDiagnoser]
    [AllStatisticsColumn]
    public class ToEntitiesComparer: ConvertRedisBase
    {
        MessagePackSerializerOptions op;
        protected override void OnSetup()
        {
            op = MessagePackSerializerOptions.Standard.WithResolver(MessagePack.Resolvers.TypelessObjectResolver.Instance);
            _ = (RedisValue)MessagePackSerializer.Serialize(a,op);
            JsonSerializer.Serialize(a);
        }
        [Benchmark]
        public void ToEntities()
        {
            for (int i = 0; i < 1; i++)
            {
                @operator.As(a);
            }
        }
        [Benchmark]
        public void AotToEntities()
        {
            for (int i = 0; i < 1; i++)
                @exoperator.As(a);
        }
        [Benchmark]
        public void MPToEntities()
        {
            for (int i = 0; i < 1; i++)
            {
                var d = (RedisValue)MessagePackSerializer.Serialize(a, op);
            }
        }
        [Benchmark]
        public void MPCacheToEntities()
        {
            for (int i = 0; i < 1; i++)
            {
                _ =mpoperator.As(a);
            }
        }
        [Benchmark]
        public void JsonToEntities()
        {
            for (int i = 0; i < 1; i++)
            {
                RedisValue str = JsonSerializer.Serialize(a);
            }
        }
        [Benchmark(Baseline = true)]
        public void RawToEntities()
        {
            for (int i = 0; i < 1; i++)
                _ = new HashEntry[]
             {
                new HashEntry("Id",a.Id),
                new HashEntry("UID",a.UID),
                new HashEntry("B.Age",a.B.Age),
                new HashEntry("B.Name",a.B.Name),
                new HashEntry("B.C.CX",a.B.C.CX),
                new HashEntry("B.C.Dx",a.B.C.Dx),
             };
        }
    }
    [MemoryDiagnoser]
    [AllStatisticsColumn]
    public class ToObjectComparer : ConvertRedisBase
    {
        private RedisValue str;
        private byte[] bs;
        MessagePackSerializerOptions op;
        protected override void OnSetup()
        {
            op = MessagePackSerializerOptions.Standard.WithResolver(MessagePack.Resolvers.TypelessObjectResolver.Instance);

            str = JsonSerializer.Serialize(a);
            JsonSerializer.Deserialize<A>(str);
            bs = MessagePack.MessagePackSerializer.Serialize(a,op);
        }
        [Benchmark]
        public void AotToObject()
        {
            for (int i = 0; i < 1; i++)
            {
                var x = new A();
                @exoperator.Write(ref x, hashEntries);
            }
        }
        [Benchmark]
        public void AotToObjectWithCreate()
        {
            for (int i = 0; i < 1; i++)
            {
                var x = (A)@exoperator.Write(hashEntries);
            }
        }
        [Benchmark]
        public void MPToObject()
        {
            for (int i = 0; i < 1; i++)
            {
                var x = MessagePack.MessagePackSerializer.Deserialize<A>(bs, op);
            }
        }
        [Benchmark]
        public void MPCacheToObject()
        {
            for (int i = 0; i < 1; i++)
            {
                var x = mpoperator.Write(mphash);
            }
        }
        [Benchmark]
        public void JsonToObject()
        {
            for (int i = 0; i < 1; i++)
                JsonSerializer.Deserialize<A>(str);
        }
        [Benchmark]
        public void ToObject()
        {
            for (int i = 0; i < 1; i++)
            {
                var x = new A();
                @operator.Write(ref x, hashEntries);
            }
        }
        [Benchmark(Baseline =true)]
        public void RawToObject()
        {
            for (int i = 0; i < 1; i++)
            {
                var map = hashEntries.ToDictionary(w => w.Name.ToString(), w => w.Value);
                var x = new A
                {
                    Id = Find<int>("Id"),
                    UID = Find<int>("UID"),
                    B = new B
                    {
                        Age = Find<int>("B.Age"),
                        Name = Find<string>("B.Name"),
                        C = new C
                        {
                            CX = Find<int>("B.C.CX"),
                            Dx = Find<string>("B.C.Dx"),
                        }

                    }
                };


                T Find<T>(string path)
                {
                    if (map.TryGetValue(path, out var val))
                    {
                        return val.Get<T>();
                    }
                    return default;
                }
            }
        }
    }

}