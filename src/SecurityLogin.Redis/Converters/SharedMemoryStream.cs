using Microsoft.IO;

namespace SecurityLogin.Redis.Converters
{
    internal static class SharedMemoryStream
    {
        public static readonly RecyclableMemoryStreamManager StreamManager=new RecyclableMemoryStreamManager();
    }
}
