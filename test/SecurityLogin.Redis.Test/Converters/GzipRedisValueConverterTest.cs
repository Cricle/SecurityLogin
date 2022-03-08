using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityLogin.Redis.Converters;
using System;
using System.Linq;

namespace SecurityLogin.Redis.Test.Converters
{
    [TestClass]
    public class GzipRedisValueConverterTest
    {
        [TestMethod]
        public void ConvertAndConvertBack()
        {
            var value = new byte[] { 1, 2, 3, 4, 5 };
            var val = GzipRedisValueConverter.Instance.Convert(null, value, null);
            var dest=(byte[])GzipRedisValueConverter.Instance.ConvertBack(val, null);
            Assert.IsTrue(value.SequenceEqual(dest));
        }
    }
}
