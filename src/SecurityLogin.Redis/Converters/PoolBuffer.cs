using System;
using System.Buffers;

namespace SecurityLogin.Redis.Converters
{
    public struct PoolBuffer:IDisposable
    {
        public readonly ArrayPool<byte> Pool;

        public readonly byte[] Buffer;

        public readonly int Length;

        internal PoolBuffer(ArrayPool<byte> pool, byte[] buffer, int length)
        {
            Pool = pool;
            Buffer = buffer;
            Length = length;
        }

        public void Dispose()
        {
            Pool.Return(Buffer);
        }
    }
}
