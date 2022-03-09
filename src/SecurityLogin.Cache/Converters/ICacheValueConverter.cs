
namespace SecurityLogin.Cache.Converters
{
    public interface ICacheValueConverter
    {
        BufferValue Convert(object instance,object value,ICacheColumn column);

        object ConvertBack(in BufferValue value, ICacheColumn column);
    }
}
