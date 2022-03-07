using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityLogin.Redis.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis.Test.Annotations
{
    [TestClass]
    public class GzipSettingAttributeTest
    {
        [TestMethod]
        public void NullEncodingName_MustUTF8()
        {
            var attr = new GzipSettingAttribute();
            Assert.AreEqual(Encoding.UTF8, attr.Encoding);
        }
        [TestMethod]
        public void GivenEncodingName_GotMustEqualsName()
        {
            var attr = new GzipSettingAttribute { EncodingName= "ASCII" };
            Assert.AreEqual(Encoding.ASCII, attr.Encoding);
        }
    }
}
