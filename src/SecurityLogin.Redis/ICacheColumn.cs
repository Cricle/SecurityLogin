﻿using StackExchange.Redis;
using Ao.ObjectDesign;
using SecurityLogin.Redis.Converters;
using System.Collections.Generic;
using System.Reflection;

namespace SecurityLogin.Redis
{
    public interface ICacheColumn
    {
        ICacheValueConverter Converter { get; }

        PropertyGetter Getter { get; }

        PropertySetter Setter { get; }

        PropertyInfo Property { get; }

        string Name { get; }

        RedisValue NameRedis { get; }

        string Path { get; }

        IReadOnlyList<ICacheColumn> Nexts { get; }
    }
}
