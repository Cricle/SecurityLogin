using StackExchange.Redis;
namespace SecurityLogin.Cache
{
    public interface IAutoWriteCache
    {
        object Write(HashEntry[] entries);
    }
}
