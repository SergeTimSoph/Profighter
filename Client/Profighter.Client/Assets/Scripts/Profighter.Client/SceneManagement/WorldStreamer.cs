using System;
using System.Collections.Generic;
using Profighter.Client.Utils;
using Profighter.Client.WorldObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Profighter.Client.SceneManagement
{
    public class WorldStreamer : MonoBehaviour
    {
        [SerializeField]
        private List<SceneInfo> sceneInfos;

        [SerializeField]
        private List<WorldObjectInfo> worldObjectInfos;

        [SerializeField]
        private WorldObjectsConfig worldObjectsConfig;

        [SerializeField]
        private Transform worldObjectsRoot;

        private Transform characterTransform;

        private readonly List<Scene> scenes = new();
        private readonly Dictionary<string, IInteractable> sceneObjects = new();

        public IDictionary<Collider, IInteractable> InteractableObjects { get; private set; } = new Dictionary<Collider, IInteractable>();

        public Transform WorldObjectsRoot => worldObjectsRoot;

        private void Start()
        {
            foreach (var sceneInfo in sceneInfos)
            {
                scenes.Add(new Scene { SceneInfo = sceneInfo, SceneStatus = SceneStatus.Unloaded });
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var scenesToLoad = new List<string>();
                foreach (var scene in scenes)
                {
                    if (PositionUtils.IsInside(scene.SceneInfo.BorderPoints, scene.SceneInfo.BorderPoints.Length, characterTransform.position))
                    {
                        //Debug.LogWarning($"Inside scene with ID: {scene.SceneInfo.ID}!");

                        scenesToLoad.Add(scene.SceneInfo.ID);
                        scenesToLoad.AddRange(scene.SceneInfo.VisibleScenes);
                    }
                    else
                    {
                        //Debug.LogWarning($"NOT Inside scene with ID: {scene.SceneInfo.ID}!");
                    }
                }

                HandleSceneLoadingAndUnloading(scenesToLoad);
            }
        }

        public void Setup(Transform characterTransform)
        {
            this.characterTransform = characterTransform;
        }

        private void HandleSceneLoadingAndUnloading(List<string> scenesToLoad)
        {
            foreach (var scene in scenes)
            {
                var shouldUnload = true;
                foreach (var sceneToLoad in scenesToLoad)
                {
                    if (sceneToLoad == scene.SceneInfo.ID)
                    {
                        shouldUnload = false;
                        if (scene.SceneStatus == SceneStatus.Unloaded)
                        {
                            scene.SceneStatus = SceneStatus.Loading;
                            var loadingProcess = SceneManager.LoadSceneAsync(scene.SceneInfo.ID, LoadSceneMode.Additive);
                            loadingProcess.completed += _ =>
                            {
                                SpawnWorldObjects(sceneToLoad);
                                scene.SceneStatus = SceneStatus.Loaded;
                            };
                        }
                    }
                }

                if (scene.SceneStatus == SceneStatus.Loaded && shouldUnload)
                {
                    scene.SceneStatus = SceneStatus.Unloading;

                    DespawnWorldObjects(scene.SceneInfo.ID);
                    var loadingProcess = SceneManager.UnloadSceneAsync(scene.SceneInfo.ID);
                    loadingProcess.completed += _ => scene.SceneStatus = SceneStatus.Unloaded;
                }
            }
        }

        private void SpawnWorldObjects(string sceneId)
        {
            foreach (var worldObjectInfo in worldObjectInfos)
            {
                if (worldObjectInfo.OriginSceneId == sceneId)
                {
                    var worldObjectConfig = worldObjectsConfig.GetWorldObjectConfig(worldObjectInfo.PrefabKey);
                    var worldGO = Instantiate(worldObjectConfig.Prefab, worldObjectInfo.OriginPosition, Quaternion.identity, worldObjectsRoot);

                    var worldObjectCollider = worldGO.GetComponent<Collider>();
                    var worldObjectRigidbody = worldGO.GetComponent<Rigidbody>();
                    var interactableObject = new InteractableObject(worldObjectInfo.Name, worldGO.transform, worldObjectCollider);

                    InteractableObjects.Add(worldObjectCollider, interactableObject);
                    sceneObjects.Add(sceneId, interactableObject);
                }
            }
        }


        private void DespawnWorldObjects(string sceneId)
        {
            foreach (var sceneObject in sceneObjects)
            {
                if (sceneObject.Key == sceneId)
                {
                    Destroy(sceneObject.Value.Transform.gameObject);
                }
            }
        }
    }

    [Serializable]
    public class SceneInfo
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private Vector3[] borderPoints;

        [SerializeField]
        private string[] visibleScenes;

        public string ID => id;

        public Vector3[] BorderPoints => borderPoints;

        public string[] VisibleScenes => visibleScenes;
    }

    public class Scene
    {
        public SceneInfo SceneInfo { get; set; }

        public SceneStatus SceneStatus { get; set; }
    }

    public enum SceneStatus
    {
        Unloading = 1,
        Unloaded = 2,
        Loading = 3,
        Loaded = 4,
    }

    [Serializable]
    public class WorldObjectInfo
    {
        [SerializeField]
        private string originSceneId;

        [SerializeField]
        private string name;

        [SerializeField]
        private string prefabKey;

        [SerializeField]
        private Vector3 originPosition;

        public string OriginSceneId => originSceneId;

        public string Name => name;

        public string PrefabKey => prefabKey;

        public Vector3 OriginPosition => originPosition;
    }

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