using StackExchange.Redis;
namespace SecurityLogin.Redis
{
    public interface IAutoWriteCache
    {
        object Write(HashEntry[] entries);
    }
}
