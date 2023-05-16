using System.Security.Cryptography;
using System.Text;
using ValueBuffer;

namespace SecurityLogin
{
    public class Md5EncryptionHelper : IEncryptionHelper
    {
        private Md5EncryptionHelper() { }

        public static readonly Md5EncryptionHelper Instance = new Md5EncryptionHelper();

        private static readonly MD5 md5 = MD5.Create();

        public byte[] ComputeHash(byte[] input)
        {
            return md5.ComputeHash(input);
        }
        public byte[] ComputeHash(string input)
        {
            using (var res = EncodingHelper.SharedEncoding(input, Encoding.UTF8))
            {
                return md5.ComputeHash(res.Buffers, 0, res.Count);
            }
        }
        public string ComputeHashToString(byte[] input)
        {
            var buffer = md5.ComputeHash(input);
            return ToHexString(buffer);
        }
        public string ComputeHashToString(string input)
        {
            var buffer = ComputeHash(input);
            return ToHexString(buffer);
        }
        private static string ToHexString(byte[] buffer)
        {
            using (var vs = new ValueStringBuilder())
            {
                for (int i = 0; i < buffer.Length; i++)
                    vs.Append(buffer[i].ToString("X2"));
                return vs.ToString();
            }
        }
    }
}
