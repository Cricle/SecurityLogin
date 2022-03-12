using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLogin.Cache.Annotations
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited =false)]
    public sealed class NotDeepAttribute:Attribute
    {
    }
}
