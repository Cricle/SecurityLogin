using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityLogin.Redis;
using SecurityLogin.Redis.Converters;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Test.Converters
{
    [TestClass]
    public class ByteArrayRedisValueConverterTest
    {
        [TestMethod]
        public void Convert()
        {
            var value = new byte[] { 1, 2, 3, 4, 5 };
            var val = (byte[])ByteArrayCacheValueConverter.Instance.Convert(null, value, null);
            Assert.IsTrue(value.SequenceEqual(val));
        }
        [TestMethod]
        public void ConvertBack()
        {
            RedisValue value = new byte[] { 1, 2, 3, 4, 5 };
            var val = ByteArrayCacheValueConverter.Instance.ConvertBack(value, null);
            Assert.IsTrue(((byte[])value).SequenceEqual((byte[])val));
        }
    }
}
