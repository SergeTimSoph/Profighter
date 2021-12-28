using System;
using UnityEngine;

namespace Profighter.Client.SceneManagement
{
    [Serializable]
    public class WorldObjectConfig
    {
        [SerializeField]
        private string prefabKey;

        [SerializeField]
        private GameObject prefab;

        public string PrefabKey => prefabKey;

        public GameObject Prefab => prefab;
    }
}