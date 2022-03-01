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
        public RedisValueConverterAttribute(Type convertType)
        {
            ConvertType = convertType ?? throw new ArgumentNullException(nameof(convertType));
        }

        public Type ConvertType { get; }
    }
}
