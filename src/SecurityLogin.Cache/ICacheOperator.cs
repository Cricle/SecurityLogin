
namespace SecurityLogin.Cache
{
    public interface ICacheOperator
    {
        void Build();

        void Write(ref object instance, BufferEntry[] entries);

        BufferEntry[] As(object value);
    }
}
