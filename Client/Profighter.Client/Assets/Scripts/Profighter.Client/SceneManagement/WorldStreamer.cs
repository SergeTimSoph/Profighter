using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<InteractableObjectInfo> interactableObjectInfos;

        [SerializeField]
        private WorldObjectsConfig worldObjectsConfig;

        [SerializeField]
        private Transform worldObjectsRoot;

        private Transform characterTransform;

        private readonly List<Scene> scenes = new();

        private readonly List<InteractableObject> interactableObjects = new();

        public List<Scene> Scenes => scenes;

        public List<InteractableObject> InteractableObjects => interactableObjects;

        public Transform WorldObjectsRoot => worldObjectsRoot;

        private void Start()
        {
            //create scenes only on first start for user. Here we are actually creating user profile with world state.
            CreateWorldModel();
        }

        private void CreateWorldModel()
        {
            foreach (var sceneInfo in sceneInfos)
            {
                scenes.Add(new Scene
                {
                    SceneInfo = sceneInfo,
                    SceneState = new SceneState(sceneInfo.Id, SceneStatus.Unloaded)
                });
            }

            foreach (var interactableObjectInfo in interactableObjectInfos)
            {
                interactableObjects.Add(new InteractableObject
                {
                    InteractableObjectInfo = interactableObjectInfo,
                    InteractableObjectState = new InteractableObjectState(interactableObjectInfo.Id, interactableObjectInfo.InitialSpawnSceneId, interactableObjectInfo.InitialPosition)
                });
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

                        scenesToLoad.Add(scene.SceneInfo.Id);
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

        public void AddSceneObject(IInteractableEntity interactableEntity, Vector3 position)
        {
            foreach (var scene in scenes)
            {
                if (PositionUtils.IsInside(scene.SceneInfo.BorderPoints, scene.SceneInfo.BorderPoints.Length, position))
                {
                    foreach (var interactableObject in interactableObjects)
                    {
                        if (interactableObject.InteractableEntity == interactableEntity)
                        {
                            interactableObject.InteractableObjectState.SceneId = scene.SceneInfo.Id;
                            interactableObject.InteractableObjectState.Position = position;
                        }
                    }
                }
            }
        }

        public void RemoveSceneObject(IInteractableEntity interactableEntity)
        {
            foreach (var interactableObject in interactableObjects)
            {
                if (interactableObject.InteractableEntity == interactableEntity)
                {
                    interactableObject.InteractableObjectState.SceneId = null;
                    return;
                }
            }
        }

        private void HandleSceneLoadingAndUnloading(List<string> scenesToLoad)
        {
            foreach (var scene in scenes)
            {
                var shouldUnload = true;
                foreach (var sceneToLoad in scenesToLoad)
                {
                    if (sceneToLoad == scene.SceneInfo.Id)
                    {
                        shouldUnload = false;
                        if (scene.SceneState.SceneStatus == SceneStatus.Unloaded)
                        {
                            scene.SceneState.SceneStatus = SceneStatus.Loading;
                            var loadingProcess = SceneManager.LoadSceneAsync(scene.SceneInfo.Id, LoadSceneMode.Additive);
                            loadingProcess.completed += _ =>
                            {
                                SpawnSceneObjects(scene.SceneInfo.Id);
                                scene.SceneState.SceneStatus = SceneStatus.Loaded;
                            };
                        }
                    }
                }

                if (scene.SceneState.SceneStatus == SceneStatus.Loaded && shouldUnload)
                {
                    scene.SceneState.SceneStatus = SceneStatus.Unloading;

                    DespawnSceneObjects(scene.SceneInfo.Id);
                    var loadingProcess = SceneManager.UnloadSceneAsync(scene.SceneInfo.Id);
                    loadingProcess.completed += _ => scene.SceneState.SceneStatus = SceneStatus.Unloaded;
                }
            }
        }

        private void SpawnSceneObjects(string sceneId)
        {
            foreach (var interactableObject in interactableObjects)
            {
                if (interactableObject.InteractableObjectState.SceneId == sceneId)
                {
                    var worldObjectConfig = worldObjectsConfig.GetWorldObjectConfig(interactableObject.InteractableObjectInfo.PrefabKey);
                    var worldGO = Instantiate(worldObjectConfig.Prefab, interactableObject.InteractableObjectState.Position, Quaternion.identity, worldObjectsRoot);

                    var worldObjectCollider = worldGO.GetComponent<Collider>();
                    var interactableEntity = new InteractableEntity(interactableObject.InteractableObjectInfo.Name, worldGO.transform, worldObjectCollider);
                    interactableObject.InteractableEntity = interactableEntity;
                }
            }
        }


        private void DespawnSceneObjects(string sceneId)
        {
            foreach (var interactableObject in interactableObjects)
            {
                if (interactableObject.InteractableObjectState.SceneId == sceneId)
                {
                    Destroy(interactableObject.InteractableEntity.Transform.gameObject);
                }
            }
        }
    }

    public class SceneState
    {
        public string Id { get; }

        public SceneStatus SceneStatus { get; set; }

        public SceneState(string id, SceneStatus sceneStatus)
        {
            Id = id;
            SceneStatus = sceneStatus;
        }
    }

    public class InteractableObjectState
    {
        public string ObjectId { get; }

        public string SceneId { get; set; }

        public Vector3 Position { get; set; }

        public InteractableObjectState(string objectId, string sceneId, Vector3 position)
        {
            ObjectId = objectId;
            SceneId = sceneId;
            Position = position;
        }
    }

    public class Scene
    {
        public SceneInfo SceneInfo { get; set; }

        public SceneState SceneState { get; set; }
    }

    public class InteractableObject
    {
        public InteractableObjectInfo InteractableObjectInfo { get; set; }

        public InteractableObjectState InteractableObjectState { get; set; }

        public IInteractableEntity InteractableEntity { get; set; }
    }

    public enum SceneStatus
    {
        Unloading = 1,
        Unloaded = 2,
        Loading = 3,
        Loaded = 4,
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

        public string Id => id;

        public Vector3[] BorderPoints => borderPoints;

        public string[] VisibleScenes => visibleScenes;
    }

    [Serializable]
    public class InteractableObjectInfo
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private string name;

        [SerializeField]
        private string prefabKey;

        [SerializeField]
        private string initialSpawnSceneId;

        [SerializeField]
        private Vector3 initialPosition;

        public string Id => id;

        public string Name => name;

        public string PrefabKey => prefabKey;

        public string InitialSpawnSceneId => initialSpawnSceneId;

        public Vector3 InitialPosition => initialPosition;
    }
}