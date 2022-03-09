namespace SecurityLogin.Cache
{
    public interface IAutoWriteCache
    {
        object Write(BufferEntry[] entries);
    }
}
