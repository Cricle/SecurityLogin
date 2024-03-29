﻿#if NETSTANDARD2_0
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
#else
using Microsoft.Extensions.ObjectPool;
#endif
using System;
using System.Security.Cryptography;
using System.Text;

namespace SecurityLogin.Mode.RSA.Helpers
{
#if NET5_0_OR_GREATER
    public static class RSA
    {
        class RSAProviderPooledObjectPolicy : PooledObjectPolicy<RSACryptoServiceProvider>
        {
            public override RSACryptoServiceProvider Create()
            {
                return new RSACryptoServiceProvider();
            }

            public override bool Return(RSACryptoServiceProvider obj)
            {
                return true;
            }
        }
        private static readonly ObjectPool<RSACryptoServiceProvider> providerPool = new DefaultObjectPool<RSACryptoServiceProvider>(new RSAProviderPooledObjectPolicy());
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
            var rsa = providerPool.Get();
            try
            {
                rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);
                var buffer = rsa.Encrypt(data, false);
                return buffer;
            }
            finally
            {
                providerPool.Return(rsa);
            }
        }
        public static byte[] DecryptByPrivateKey(byte[] data, string privateKey)
        {
            var rsa = providerPool.Get();
            try
            {
                rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);
                var buffer = rsa.Decrypt(data, false);
                return buffer;
            }
            finally
            {
                providerPool.Return(rsa);
            }
        }
        public static byte[] EncryptByPublicKey(byte[] data, string publicKey)
        {
            var rsa = providerPool.Get();
            try
            {
                rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
                var buffer = rsa.Encrypt(data, false);
                return buffer;

            }
            finally
            {
                providerPool.Return(rsa);
            }
        }
    }
#else
    public static partial class RSA
    {
        private static readonly Encoding UTF8 = Encoding.UTF8;

        public static RSAKey GetKey(int keyLen = 1024)
        {
            var keyGenerator = new RsaKeyPairGenerator();

            var param = new RsaKeyGenerationParameters(
                BigInteger.ValueOf(3),
                new SecureRandom(),
                keyLen, 25);
            keyGenerator.Init(param);
            var keyPair = keyGenerator.GenerateKeyPair();
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

        public static byte[] EncryptByPrivateKey(byte[] data, string privateKey)
        {
            var engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(true, GetPrivateKeyParameter(privateKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }

        public static byte[] DecryptByPrivateKey(byte[] data, string privateKey)
        {
            data = UTF8.GetBytes(UTF8.GetString(data).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty));
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(false, GetPrivateKeyParameter(privateKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }

        public static byte[] EncryptByPublicKey(byte[] data, string publicKey)
        {
            var engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(true, GetPublicKeyParameter(publicKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }

        public static byte[] DecryptByPublicKey(byte[] data, string publicKey)
        {
            data = UTF8.GetBytes(UTF8.GetString(data).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty));
            var engine = new Pkcs1Encoding(new RsaEngine());

            engine.Init(false, GetPublicKeyParameter(publicKey));
            var ResultData = engine.ProcessBlock(data, 0, data.Length);
            return ResultData;
        }
    }
#endif

}
