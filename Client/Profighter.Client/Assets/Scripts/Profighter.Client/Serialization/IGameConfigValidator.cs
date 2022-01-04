using Profighter.Client.Configuration;

namespace Profighter.Client.Serialization
{
    public interface IGameConfigValidator
    {
        GameConfig Validate(string filePath);
    }
}