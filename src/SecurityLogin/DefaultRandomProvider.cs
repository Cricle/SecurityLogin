using System;
using System.Security.Cryptography;

namespace SecurityLogin
{
    public class DefaultRandomProvider : IRandomProvider
    {
        public static readonly DefaultRandomProvider Instance = new DefaultRandomProvider();

        private DefaultRandomProvider() { }

#if !NET6_0_OR_GREATER
        private static readonly RandomNumberGenerator generator = RandomNumberGenerator.Create();
#endif
        public int GetRandom(int min, int max)
        {
#if NET6_0_OR_GREATER
            return Random.Shared.Next(min, max);
#else
            var buffer = new byte[sizeof(int)];
            generator.GetBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
#endif
        }
    }
}
