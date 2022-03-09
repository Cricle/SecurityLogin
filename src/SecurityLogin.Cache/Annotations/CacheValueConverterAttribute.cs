using SecurityLogin.Cache.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Cache.Annotations
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited =false)]
    public sealed class CacheValueConverterAttribute:Attribute
    {
        private static readonly string CacheValueConverterName = typeof(ICacheValueConverter).FullName;

        public CacheValueConverterAttribute(Type convertType)
        {
            ConvertType = convertType ?? throw new ArgumentNullException(nameof(convertType));
            if (convertType.GetInterface(CacheValueConverterName)==null)
            {
                throw new ArgumentException($"Type {convertType} is not implement {CacheValueConverterName}");
            }
        }

        public Type ConvertType { get; }
    }
}
