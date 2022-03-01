using StackExchange.Redis;

namespace SecurityLogin.Redis.Converters
{
    public interface IRedisValueConverter
    {
        RedisValue Convert(object instance,object value,IRedisColumn column);

        object ConvertBack(in RedisValue value, IRedisColumn column);
    }
}
