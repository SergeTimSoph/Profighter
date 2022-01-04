using System.Collections.Generic;
using UnityEngine;

namespace Profighter.Client.Configuration
{
    public class AreaConfig
    {
        public string Id { get; set; }

        public string SceneName { get; set; }

        public IEnumerable<Vector3> BorderPoints { get; set; }

        public IEnumerable<string> VisibleAreas { get; set; }
    }
}