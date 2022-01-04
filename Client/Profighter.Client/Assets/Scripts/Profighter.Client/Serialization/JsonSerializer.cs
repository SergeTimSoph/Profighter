using Newtonsoft.Json;

namespace Profighter.Client.Serialization
{
    public class JsonSerializer : ISerializer
    {
        public T DeserializeObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}