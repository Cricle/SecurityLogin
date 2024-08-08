using Microsoft.AspNetCore.Http;
using System;

namespace SecurityLogin.AspNetCore
{
    internal static class Throws
    {
        public static void ThrowHttpContextIsNull(HttpContext? context)
        {
            if (context == null)
            {
                throw new InvalidOperationException("The HttpContext is null");
            }
        }
    }
}
