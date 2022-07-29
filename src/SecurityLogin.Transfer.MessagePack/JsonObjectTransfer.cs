using System;
using System.Text;
using Ao.Cache;
using global::MessagePack;
using global::MessagePack.Resolvers;

namespace SecurityLogin.Transfer.MessagePack
{
    public class JsonObjectTransfer : IObjectTransfer
    {
        private static readonly MessagePackSerializerOptions defaultOptions = MessagePackSerializerOptions.Standard.WithResolver(TypelessObjectResolver.Instance)
            .WithCompression(MessagePackCompression.Lz4Block);

        public JsonObjectTransfer()
            : this(defaultOptions)
        {

        }
        public JsonObjectTransfer(MessagePackSerializerOptions options)
            : this(options, Encoding.UTF8)
        {

        }
        public JsonObjectTransfer(MessagePackSerializerOptions options, Encoding encoding)
        {
            Options = options;
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public MessagePackSerializerOptions Options { get; }

        public Encoding Encoding { get; }

        public byte[] Transfer<T>(T obj)
        {
            return MessagePackSerializer.Serialize(typeof(T), obj, Options);
        }

        public T Transfer<T>(byte[] data)
        {
            return (T)MessagePackSerializer.Deserialize(typeof(T), data, Options);
        }

        public T TransferByString<T>(string data)
        {
            using (var bytes = EncodingHelper.SharedEncoding(data, Encoding))
            {
                return (T)MessagePackSerializer.Deserialize(typeof(T),
                    new ReadOnlyMemory<byte>(bytes.Buffers, 0, bytes.Count),
                    Options);
            }
        }

        public string TransferToString<T>(T obj)
        {
            var bs = Transfer(obj);
            return Encoding.GetString(bs);
        }
    }
}
