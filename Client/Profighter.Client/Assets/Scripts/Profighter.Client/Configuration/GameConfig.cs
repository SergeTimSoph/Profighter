using System.Collections.Generic;

namespace Profighter.Client.Configuration
{
    public class GameConfig
    {
        public IEnumerable<AreaConfig> AreasConfig { get; set; }

        public IEnumerable<AreaItemsConfig> InitialAreaItemsConfig { get; set; }
    }
}