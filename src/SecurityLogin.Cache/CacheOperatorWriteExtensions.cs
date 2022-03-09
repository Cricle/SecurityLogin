namespace SecurityLogin.Cache
{
    public static class CacheOperatorWriteExtensions
    {
        public static T Create<T>(this ComplexCacheOperator @operator, BufferEntry[] entries)
        {
            return (T)Create(@operator,entries);
        }
        public static object Create(this ComplexCacheOperator @operator, BufferEntry[] entries)
        {
            var inst = @operator.Create();
            @operator.Write(ref inst, entries);
            return inst;
        }
        public static void Write<T>(this ICacheOperator @operator,ref T instance, BufferEntry[] entries)
        {
            object val = instance;
            @operator.Write(ref val, entries);
            instance = (T)val;
        }
    }
}
