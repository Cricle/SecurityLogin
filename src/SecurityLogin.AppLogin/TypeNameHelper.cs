using System;
using System.Collections.Generic;

namespace SecurityLogin.AppLogin
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
            n= string.Concat(type.FullName.Split('`')[0], GetFriendlyName(type));
            friendlyName[type] = n;
            return n;
        }
        public static string GetFriendlyName(Type type)
        {
            if (type.IsGenericType)
            {
                var gens = type.GetGenericArguments();
                var names = new string[gens.Length];
                for (int i = 0; i < gens.Length; i++)
                {
                    var gen = gens[i];
                    names[i] = GetFriendlyName(gen);
                }
                var actualName = type.Name.Split('`')[0];
                return string.Concat(actualName, "<", string.Join(",", names), ">");
            }
            return type.Name;
        }
    }
}
