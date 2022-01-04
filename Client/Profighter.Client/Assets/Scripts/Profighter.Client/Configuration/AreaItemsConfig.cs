using System.Collections.Generic;

namespace Profighter.Client.Configuration
{
    public class AreaItemsConfig
    {
        public string AreaId { get; set; }

        public IEnumerable<AreaItemConfig> AreaItems { get; set; }
    }
}