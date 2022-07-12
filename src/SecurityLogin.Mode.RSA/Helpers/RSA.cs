#if NETSTANDARD2_0
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
#endif
using System;
using System.Security.Cryptography;
using System.Text;

namespace SecurityLogin.Mode.RSA.Helpers
{
#if NET5_0_OR_GREATER
    public static class RSA
    {
        public static readonly Encoding UTF8 = Encoding.UTF8;
        public static RSAKey GetKey(int keyLen = 1024)
        {
            using var rsa = new RSACryptoServiceProvider(keyLen);
            var privateKey = rsa.ExportPkcs8PrivateKey();
            var publicKey = rsa.ExportSubjectPublicKeyInfo();
            var pub = Convert.ToBase64String(publicKey);
            var pri = Convert.ToBase64String(privateKey);
            return new RSAKey(pub, pri);
        }
        public static byte[] EncryptByPrivateKey(byte[] data, string privateKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);
            var buffer = rsa.Encrypt(data, false);
            return buffer;
        }
        public static byte[] DecryptByPrivateKey(byte[] data, string privateKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);
            var buffer = rsa.Decrypt(data, false);
            return buffer;
        }
        public static byte[] EncryptByPublicKey(byte[] data, string publicKey)
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
            var buffer = rsa.Encrypt(data, false);
            return buffer;

        }
    }
#else
    public static partial class RSA
    {
        private static readonly Encoding UTF8 = Encoding.UTF8;

        public static RSAKey GetKey(int keyLen = 1024)
        {
            //RSA密钥对的构造器  
            var keyGenerator = new RsaKeyPairGenerator();

            //RSA密钥构造器的参数  
            var param = new RsaKeyGenerationParameters(
                BigInteger.ValueOf(3),
                new SecureRandom(),
                keyLen,   //密钥长度  
                25);
            //用参数初始化密钥构造器  
            keyGenerator.Init(param);
            //产生密钥对  
            var keyPair = keyGenerator.GenerateKeyPair();
            //获取公钥和密钥  
            var publicKey = keyPair.Public;
            var privateKey = keyPair.Private;

            var subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);


            var asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();

            var publicInfoByte = asn1ObjectPublic.GetEncoded("UTF-8");
            var asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            var privateInfoByte = asn1ObjectPrivate.GetEncoded("UTF-8");

            var pub = Convert.ToBase64String(publicInfoByte);
            var pri = Convert.ToBase64String(privateInfoByte);

            var item = new RSAKey(pub, pri);
            return item;
        }
        private static AsymmetricKeyParameter GetPublicKeyParameter(string keyBase64)
        {
            keyBase64 = keyBase64.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
            var publicInfoByte = Convert.FromBase64String(keyBase64);
            var pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }

        private static AsymmetricKeyParameter GetPrivateKeyParameter(string keyBase64)
        {
            keyBase64 = keyBase64.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
            var privateInfoByte = Convert.FromBase64String(keyBase64);
            var priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
            return priKey;
        }

        /// <summary>
        /// 私钥加密
        /// </summary>
        /// <param name="data">加密内容</param>
        /// <param name="privateKey">私钥（Base64后的）</param>
        /// <returns>返回Base64内容</returns>
        public static byte[] EncryptByPrivateKey(byte[] data, string privateKey)
        {
            //非对称加密算法，加解密用  
            var engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(true, GetPrivateKeyParameter(privateKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }

        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="data">待解密的内容</param>
        /// <param name="privateKey">私钥（Base64编码后的）</param>
        /// <returns>返回明文</returns>
        public static byte[] DecryptByPrivateKey(byte[] data, string privateKey)
        {
            data = UTF8.GetBytes(UTF8.GetString(data).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty));
            //非对称加密算法，加解密用  
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(false, GetPrivateKeyParameter(privateKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }

        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="data">加密内容</param>
        /// <param name="publicKey">公钥（Base64编码后的）</param>
        /// <returns>返回Base64内容</returns>
        public static byte[] EncryptByPublicKey(byte[] data, string publicKey)
        {
            //非对称加密算法，加解密用  
            var engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(true, GetPublicKeyParameter(publicKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }

        /// <summary>
        /// 公钥解密
        /// </summary>
        /// <param name="data">待解密的内容</param>
        /// <param name="publicKey">公钥（Base64编码后的）</param>
        /// <returns>返回明文</returns>
        public static byte[] DecryptByPublicKey(byte[] data, string publicKey)
        {
            data = UTF8.GetBytes(UTF8.GetString(data).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty));
            //非对称加密算法，加解密用  
            var engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(false, GetPublicKeyParameter(publicKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }
    }
#endif

}
