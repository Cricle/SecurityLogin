using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SecurityLogin.Transfer.TextJson
{
    public class JsonObjectTransfer : IObjectTransfer
    {
        public JsonObjectTransfer(JsonSerializerOptions options, Encoding encoding)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public JsonSerializerOptions Options { get; }

        public Encoding Encoding { get; }

        public byte[] Transfer<T>(T obj)
        {
            var str=TransferToString(obj);
            return Encoding.GetBytes(str);
        }

        public T Transfer<T>(byte[] data)
        {
            var str=Encoding.GetString(data);
            return TransferByString<T>(str);
        }

        public T TransferByString<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data, Options);
        }

        public string TransferToString<T>(T obj)
        {
           return JsonSerializer.Serialize(obj,Options);
        }
    }
}
