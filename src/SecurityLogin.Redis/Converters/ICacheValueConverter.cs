﻿using StackExchange.Redis;

namespace SecurityLogin.Cache.Converters
{
    public interface ICacheValueConverter
    {
        RedisValue Convert(object instance,object value,ICacheColumn column);

        object ConvertBack(in RedisValue value, ICacheColumn column);
    }
}