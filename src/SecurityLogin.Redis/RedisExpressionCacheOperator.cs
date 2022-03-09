using SecurityLogin.Cache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Redis
{
    public static class RedisCacheOperatorExtensions
    {
        private static readonly HashEntry[] emptryHashEntry = new HashEntry[0];
        public static HashEntry[] AsEnrty(this BufferEntry[] entries)
        {
#if NET6_0_OR_GREATER
            var res = GC.AllocateUninitializedArray<HashEntry>(entries.Length);
#else
            var res = new HashEntry[entries.Length];
#endif
            for (int i = 0; i < entries.Length; i++)
            {
                var item = entries[i];
                res[i] = new HashEntry((byte[])item.Name, (byte[])item.Value);
            }
            return res;
        }
        public static BufferEntry[] AsBuffer(this HashEntry[] entries)
        {
#if NET6_0_OR_GREATER
            var res = GC.AllocateUninitializedArray<BufferEntry>(entries.Length);
#else
            var res = new BufferEntry[entries.Length];
#endif
            for (int i = 0; i < entries.Length; i++)
            {
                var item = entries[i];
                res[i] = new BufferEntry((byte[])item.Name, (byte[])item.Value);
            }
            return res;
        }
        public static HashEntry[] AsRedis(this ICacheOperator @operator,object value)
        {
            var entry = @operator.As(value);
            if (entry==null||entry.Length==0)
            {
                return emptryHashEntry;
            }
            return AsEnrty(entry);
        }
        public static void WriteRedis(this ICacheOperator @operator, ref object instance, HashEntry[] entries)
        {
            var res = AsBuffer(entries);
            @operator.Write(ref instance, res);
        }

    }
}
