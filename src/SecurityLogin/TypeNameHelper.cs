using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SecurityLogin
{
    public static class TypeNameHelper
    {
        private static readonly Dictionary<Type, string> friendlyName = new Dictionary<Type, string>();

        public static string GetFriendlyFullName(Type type)
        {
            if (friendlyName.TryGetValue(type, out var n))
            {
                return n;
            }
            n = string.Concat(GetGenericName(type.Name), GetFriendlyName(type));
            friendlyName[type] = n;
            return n;
        }
        private static string GetFriendlyName(Type type)
        {
            var gens = type.GenericTypeArguments;
            if (gens != null && gens.Length != 0)
            {
                var names = new string[gens.Length];
                for (int i = 0; i < gens.Length; i++)
                {
                    names[i] = GetFriendlyName(gens[i]);
                }
                var actualName = GetGenericName(type.Name);
                return string.Concat(actualName, "<", string.Join(",", names), ">");
            }
            return type.Name;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetGenericName(string name)
        {
            var index = name.IndexOf('`');
            if (index == -1)
            {
                return name;
            }
            return name.Substring(0, index);
        }
    }
}
