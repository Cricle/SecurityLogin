using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityLogin.Redis;
using SecurityLogin.Redis.Annotations;
using SecurityLogin.Redis.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Test.Annotations
{
    internal class NullCacheValueConverter : ICacheValueConverter
    {
        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return default;
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            return null;
        }
    }
    [TestClass]
    public class RedisValueConverterAttributeTest
    {
        [TestMethod]
        public void GivenNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CacheValueConverterAttribute(null));
        }
        [TestMethod]
        public void GivenNotImplmentIRedisValueConverter_MustThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => new CacheValueConverterAttribute(typeof(object)));
        }
        [TestMethod]
        public void GivenType_PropertyMustEqualInput()
        {
            var t = typeof(NullCacheValueConverter);
            var attr = new CacheValueConverterAttribute(t);
            Assert.AreEqual(t, attr.ConvertType);
        }
    }
}
