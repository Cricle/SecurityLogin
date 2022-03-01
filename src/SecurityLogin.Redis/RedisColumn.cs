using Ao.ObjectDesign;
using SecurityLogin.Redis.Converters;
using StackExchange.Redis;
using System.Reflection;

namespace SecurityLogin.Redis
{
    internal class RedisColumn : IRedisColumn
    {
        public IRedisValueConverter Converter { get; set; }

        public PropertyGetter Getter { get; set; }

        public PropertySetter Setter { get; set; }

        public PropertyInfo Property { get; set; }

        public string Name { get; set; }

        public RedisValue NameRedis { get; set; }
    }
}
