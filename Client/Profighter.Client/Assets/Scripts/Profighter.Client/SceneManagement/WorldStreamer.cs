using System;
using System.Collections.Generic;
using System.Linq;
using Profighter.Client.Configuration;
using Profighter.Client.Data;
using Profighter.Client.Entities;
using Profighter.Client.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Profighter.Client.SceneManagement
{
    public class WorldStreamer : MonoBehaviour
    {
        [SerializeField]
        private WorldObjectsConfig worldObjectsConfig;

        [SerializeField]
        private Transform worldObjectsRoot;

        private readonly Dictionary<string, AreaLoadingStatus> areaLoadingStatuses = new();
        private readonly Dictionary<string, List<AreaObject>> areaObjects = new();
        private Transform characterTransform;
        private World world;

        private GameConfig gameConfig;

        public World World => world;

        public Transform WorldObjectsRoot => worldObjectsRoot;

        public Dictionary<string, List<AreaObject>> AreaObjects => areaObjects;

        private void InitializeWorld()
        {
            world = new World(
                new WorldState(), gameConfig);

            foreach (var areaConfig in gameConfig.AreasConfig)
            {
                var area = world.AddArea(areaConfig.Id, areaConfig);
                areaLoadingStatuses.Add(area.Id, AreaLoadingStatus.Unloaded);
                areaObjects.Add(area.Id, new List<AreaObject>());

                var initialAreaItemsConfig = gameConfig.InitialAreaItemsConfig
                    .FirstOrDefault(x => x.AreaId == area.Id);

                if (initialAreaItemsConfig != null)
                {
                    foreach (var areaItem in initialAreaItemsConfig.AreaItems)
                    {
                        area.PutItem(areaItem.Identity, areaItem.Position);
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var areasToLoad = new List<string>();
                foreach (var areaConfig in gameConfig.AreasConfig)
                {
                    if (PositionUtils.IsInside(areaConfig.BorderPoints.ToArray(), areaConfig.BorderPoints.Count(), characterTransform.position))
                    {
                        areasToLoad.Add(areaConfig.Id);
                        areasToLoad.AddRange(areaConfig.VisibleAreas);
                    }
                }

                HandleAreasLoadingAndUnloading(areasToLoad);
            }
        }

        public void Setup(Transform characterTransform, GameConfig gameConfig)
        {
            this.characterTransform = characterTransform;
            this.gameConfig = gameConfig;

            InitializeWorld();
        }

        private void HandleAreasLoadingAndUnloading(List<string> areasToLoad)
        {
            foreach (var areaConfig in gameConfig.AreasConfig)
            {
                var shouldUnload = true;
                foreach (var areaToLoad in areasToLoad)
                {
                    if (areaToLoad == areaConfig.Id)
                    {
                        shouldUnload = false;
                        if (areaLoadingStatuses[areaConfig.Id] == AreaLoadingStatus.Unloaded)
                        {
                            areaLoadingStatuses[areaConfig.Id] = AreaLoadingStatus.Loading;
                            var loadingProcess = SceneManager.LoadSceneAsync(areaConfig.SceneName, LoadSceneMode.Additive);
                            loadingProcess.completed += _ =>
                            {
                                SpawnAreaObjects(areaConfig.Id);
                                areaLoadingStatuses[areaConfig.Id] = AreaLoadingStatus.Loaded;
                            };
                        }
                    }
                }

                if (areaLoadingStatuses[areaConfig.Id] == AreaLoadingStatus.Loaded && shouldUnload)
                {
                    areaLoadingStatuses[areaConfig.Id] = AreaLoadingStatus.Unloading;

                    DespawnAreaObjects(areaConfig.Id);
                    var loadingProcess = SceneManager.UnloadSceneAsync(areaConfig.Id);
                    loadingProcess.completed += _ => areaLoadingStatuses[areaConfig.Id] = AreaLoadingStatus.Unloaded;
                }
            }
        }

        private void SpawnAreaObjects(string areaId)
        {
            var area = world.GetArea(areaId);

            foreach (var areaItemStack in area.Items)
            {
                WorldObjectConfig areaObjectConfig;

                switch (areaItemStack.Item)
                {
                    case FoodIdentity foodIdentity:
                        areaObjectConfig = worldObjectsConfig.GetWorldObjectConfig(foodIdentity.FoodId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var areaGO = Instantiate(areaObjectConfig.Prefab, areaItemStack.Position, Quaternion.identity, worldObjectsRoot);
                var areaObjectCollider = areaGO.GetComponent<Collider>();
                areaObjects.TryGetValue(areaId, out var objects);
                objects.Add(new AreaObject
                {
                    AreaGameObject = areaGO,
                    AreaObjectCollider = areaObjectCollider,
                    ItemIdentity = areaItemStack.Item,
                    AreaId = area.Id
                });
            }
        }


        private void DespawnAreaObjects(string areaId)
        {
            areaObjects.TryGetValue(areaId, out var objects);

            foreach (var areaObject in objects)
            {
                Destroy(areaObject.AreaGameObject);
            }

            objects.Clear();
        }

        public void AddAreaObject(CharacterHoldObject currentInventoryItem, Vector3 position)
        {
            foreach (var area in world.Areas)
            {
                if (PositionUtils.IsInside(area.AreaConfig.BorderPoints.ToArray(), area.AreaConfig.BorderPoints.Count(), position))
                {
                    area.PutItem(currentInventoryItem.ItemIdentity, position);

                    areaObjects.TryGetValue(area.Id, out var objects);
                    objects.Add(new AreaObject
                    {
                        AreaGameObject = currentInventoryItem.GameObject,
                        AreaObjectCollider = currentInventoryItem.Collider,
                        ItemIdentity = currentInventoryItem.ItemIdentity,
                        AreaId = area.Id
                    });

                }
            }
        }
    }

    public class AreaObject
    {
        public IItemIdentity ItemIdentity { get; set; }

        public string AreaId { get; set; }

        public GameObject AreaGameObject { get; set; }

        public Collider AreaObjectCollider { get; set; }
    }

    public enum AreaLoadingStatus
    {
        Unloading = 1,
        Unloaded = 2,
        Loading = 3,
        Loaded = 4,
    }
}