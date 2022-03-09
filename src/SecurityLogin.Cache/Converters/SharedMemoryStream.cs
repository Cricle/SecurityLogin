using Microsoft.IO;

namespace SecurityLogin.Cache.Converters
{
    internal static class SharedMemoryStream
    {
        public static readonly RecyclableMemoryStreamManager StreamManager=new RecyclableMemoryStreamManager();
    }
}
