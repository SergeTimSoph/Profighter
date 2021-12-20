using System;
using System.Collections.Generic;
using System.Linq;
using Profighter.Client.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Profighter.Client.SceneManagement
{
    public class WorldStreamer : MonoBehaviour
    {
        [SerializeField]
        private List<SceneInfo> sceneInfos;

        private Transform characterTransform;

        private readonly List<Scene> scenes = new();

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
                        Debug.LogWarning($"Inside scene with ID: {scene.SceneInfo.ID}!");

                        scenesToLoad.Add(scene.SceneInfo.ID);
                        scenesToLoad.AddRange(scene.SceneInfo.VisibleScenes);
                    }
                    else
                    {
                        Debug.LogWarning($"NOT Inside scene with ID: {scene.SceneInfo.ID}!");
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
                            loadingProcess.completed += _ => scene.SceneStatus = SceneStatus.Loaded;
                        }
                    }
                }

                if (scene.SceneStatus == SceneStatus.Loaded && shouldUnload)
                {
                    scene.SceneStatus = SceneStatus.Unloading;
                    var loadingProcess = SceneManager.UnloadSceneAsync(scene.SceneInfo.ID);
                    loadingProcess.completed += _ => scene.SceneStatus = SceneStatus.Unloaded;
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
}