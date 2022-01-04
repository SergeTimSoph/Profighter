namespace Profighter.Client.Serialization
{
    public interface ISerializer
    {
        T DeserializeObject<T>(string data);
    }
}