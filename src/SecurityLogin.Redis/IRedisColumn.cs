using Ao.ObjectDesign;
using SecurityLogin.Redis.Converters;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Reflection;

namespace SecurityLogin.Redis
{
    public interface IRedisColumn
    {
        IRedisValueConverter Converter { get; }

        PropertyGetter Getter { get; }

        PropertySetter Setter { get; }

        PropertyInfo Property { get; }

        string Name { get; }

        RedisValue NameRedis { get; }

        string Path { get; }

        IReadOnlyList<IRedisColumn> Nexts { get; }
    }
}
