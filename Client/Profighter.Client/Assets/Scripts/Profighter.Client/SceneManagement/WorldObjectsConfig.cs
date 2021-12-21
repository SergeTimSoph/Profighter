using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Profighter.Client.SceneManagement
{
    [CreateAssetMenu(fileName = "WorldObjectsConfig", menuName = "Profighter/WorldObjectsConfig")]
    public class WorldObjectsConfig : ScriptableObject
    {
        [SerializeField]
        private List<WorldObjectConfig> worldObjectConfigs;

        public WorldObjectConfig GetWorldObjectConfig(string key)
        {
            var worldObjectConfig = worldObjectConfigs.FirstOrDefault(x => x.PrefabKey == key);

            if (worldObjectConfig == null)
            {
                Debug.LogError($"Can't find world object config for object key {key}");
            }

            return worldObjectConfig;
        }
    }
}