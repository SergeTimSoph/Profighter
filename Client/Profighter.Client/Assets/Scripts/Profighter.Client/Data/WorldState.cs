using System.Collections.Generic;

namespace Profighter.Client.Data
{
    public class WorldState
    {
        public List<AreaState> Areas { get; } = new();
    }
}