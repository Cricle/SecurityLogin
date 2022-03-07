using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    internal class NullRedisValueConverter : IRedisValueConverter
    {
        public RedisValue Convert(object instance, object value, IRedisColumn column)
        {
            return default;
        }

        public object ConvertBack(in RedisValue value, IRedisColumn column)
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
            Assert.ThrowsException<ArgumentNullException>(() => new RedisValueConverterAttribute(null));
        }
        [TestMethod]
        public void GivenNotImplmentIRedisValueConverter_MustThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => new RedisValueConverterAttribute(typeof(object)));
        }
        [TestMethod]
        public void GivenType_PropertyMustEqualInput()
        {
            var t = typeof(NullRedisValueConverter);
            var attr = new RedisValueConverterAttribute(t);
            Assert.AreEqual(t, attr.ConvertType);
        }
    }
}
