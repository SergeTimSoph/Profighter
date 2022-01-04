using System.Collections.Generic;

namespace Profighter.Client.Data
{
    public class AreaState
    {
        public string Id { get; set; }

        public List<AreaItemStackState> AreaItems { get; } = new();
    }
}