using System.Collections.Generic;
using System.Linq;
using Profighter.Client.Configuration;
using Profighter.Client.Data;

namespace Profighter.Client.Entities
{
    public class World
    {
        private readonly WorldState worldState;
        private readonly GameConfig gameConfig;

        public IReadOnlyList<Area> Areas => worldState.Areas.Select(_ => new Area(_, gameConfig.AreasConfig.FirstOrDefault(x => x.Id == _.Id))).ToList();

        public World(WorldState worldState, GameConfig gameConfig)
        {
            this.worldState = worldState;
            this.gameConfig = gameConfig;
        }

        public Area AddArea(string areaId, AreaConfig areaConfig)
        {
            var newAreaState = new AreaState()
            {
                Id = areaId
            };

            worldState.Areas.Add(newAreaState);
            return new Area(newAreaState, areaConfig);
        }

        public Area GetArea(string areaId)
            => Areas.FirstOrDefault(x => x.Id == areaId);
    }
}