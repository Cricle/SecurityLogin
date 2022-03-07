using SecurityLogin.Redis.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Annotations
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited =false)]
    public sealed class RedisValueConverterAttribute:Attribute
    {
        private static readonly string RedisValueConverterName = typeof(IRedisValueConverter).FullName;

        public RedisValueConverterAttribute(Type convertType)
        {
            ConvertType = convertType ?? throw new ArgumentNullException(nameof(convertType));
            if (convertType.GetInterface(RedisValueConverterName)==null)
            {
                throw new ArgumentException($"Type {convertType} is not implement {RedisValueConverterName}");
            }
        }

        public Type ConvertType { get; }
    }
}
