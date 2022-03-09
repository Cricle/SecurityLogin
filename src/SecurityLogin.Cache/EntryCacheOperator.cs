using Ao.ObjectDesign;
using SecurityLogin.Cache.Converters;
using System;
using System.Collections.Generic;

namespace SecurityLogin.Cache
{
    public abstract class EntryCacheOperator : ICacheOperator, IAutoWriteCache
    {
        public static readonly BufferValue defaultName = new BufferValue("Default");

        private bool isValueType;
        private bool isString;
        private bool isObject;
        private TypeCreator typeCreator;

        public Type Target { get; }

        public EntryCacheOperator(Type target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public virtual void Build()
        {
            isValueType = Target.IsValueType;
            isString = Target == typeof(string);
            isObject = !isString && Target.IsClass;
            if (isObject)
            {
                typeCreator = CompiledPropertyInfo.GetCreator(Target);
            }
        }


        public BufferEntry[] As(object value)
        {
            return new BufferEntry[]
            {
                new BufferEntry(defaultName, AsCore(value))
            };
        }

        public void Write(ref object instance, BufferEntry[] entries)
        {
            if (entries.Length != 0)
            {
                WriteCore(ref instance, entries[0].value);
            }
        }
        public object Write(BufferEntry[] entries)
        {
            var instance = CreateInstance();
            Write(ref instance, entries);
            return instance;
        }
        protected abstract void WriteCore(ref object instance,in BufferValue entry);
        protected abstract BufferValue AsCore(object value);

        protected virtual object CreateInstance()
        {
            if (isValueType)
            {
                if (!structCache.TryGetValue(Target, out var val))
                {
                    val = Activator.CreateInstance(Target);
                    structCache[Target] = val;
                }
                return val;
            }
            if (isString)
            {
                return string.Empty;
            }
            return typeCreator?.Invoke();
        }

        private static readonly Dictionary<Type, object> structCache = new Dictionary<Type, object>();
    }
}
