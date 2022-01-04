using System.Collections.Generic;

namespace Profighter.Client.Configuration
{
    public interface IGameConfigProvider
    {
        GameConfig GetGameConfig();
    }
}