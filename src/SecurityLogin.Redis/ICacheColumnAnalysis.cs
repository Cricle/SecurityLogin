using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityLogin.Redis
{
    public interface ICacheColumnAnalysis
    {
        IReadOnlyDictionary<string, ICacheColumn> GetRedisColumnMap(Type type, string prefx);

        IReadOnlyList<ICacheColumn> GetRedisColumns(Type type, string prefx);
    }
}
