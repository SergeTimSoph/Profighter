using System;
using System.Collections.Generic;
using Profighter.Client.Camera;
using Profighter.Client.Data;
using Profighter.Client.Entities;
using Profighter.Client.PlayerInput;
using Profighter.Client.SceneManagement;
using Profighter.Client.WorldObjects;
using UnityEngine;

namespace Profighter.Client.Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private CharacterInputController inputController;

        [SerializeField]
        private CharacterController characterController;

        [SerializeField]
        private LayerMask raycastMask;

        [SerializeField]
        private Transform holdObjectRoot;

        private readonly RaycastHit[] raycastHits = new RaycastHit[1];

        private bool isSetup;
        private Inventory inventory;
        private OrbitCamera orbitCamera;

        private AreaObject currentAreaObject;
        private Transform worldObjectsRoot;
        private WorldStreamer worldStreamer;
        private World world;

        private void FixedUpdate()
        {
            if (!isSetup)
            {
                return;
            }

            var cameraTransform = orbitCamera.transform;
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
            var hits = Physics.RaycastNonAlloc(ray, raycastHits, 50f, raycastMask);
            Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, 0.5f);

            if (hits != 0)
            {
                var isFound = TryGetAreaObject(raycastHits[0].collider, out var areaObject);

                if (isFound)
                {
                    currentAreaObject = areaObject;
                }
                else
                {
                    currentAreaObject = null;
                }
            }
            else
            {
                currentAreaObject = null;
            }
        }

        private bool TryGetAreaObject(Collider collider, out AreaObject resultAreaObject)
        {
            foreach (var areaObjectPair in worldStreamer.AreaObjects)
            {
                foreach (var areaObject in areaObjectPair.Value)
                {
                    if (areaObject.AreaObjectCollider == collider)
                    {
                        resultAreaObject = areaObject;
                        return true;
                    }
                }
            }

            resultAreaObject = null;
            return false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && currentAreaObject != null)
            {
                var area = world.GetArea(currentAreaObject.AreaId);
                area.TakeAllItems(currentAreaObject.ItemIdentity);
                worldStreamer.AreaObjects.TryGetValue(area.Id, out var areaObjects);
                areaObjects.Remove(currentAreaObject);

                var holdObject = new CharacterHoldObject()
                {
                    ItemIdentity = currentAreaObject.ItemIdentity,
                    Collider = currentAreaObject.AreaObjectCollider,
                    GameObject = currentAreaObject.AreaGameObject
                };
                inventory.SetCurrentItem(holdObject);
                characterController.IgnoredColliders.Add(currentAreaObject.AreaObjectCollider);
                currentAreaObject.AreaGameObject.transform.parent = holdObjectRoot;
                currentAreaObject.AreaGameObject.transform.position = holdObjectRoot.position;
                currentAreaObject.AreaGameObject.transform.rotation = holdObjectRoot.rotation;
            }
            else if (Input.GetKeyDown(KeyCode.Y) && inventory.GetCurrentItem() != null)
            {
                var currentInventoryItem = inventory.GetCurrentItem();
                characterController.IgnoredColliders.Remove(currentInventoryItem.Collider);
                currentInventoryItem.GameObject.transform.parent = worldObjectsRoot;
                inventory.RemoveCurrentItem();
                worldStreamer.AddAreaObject(currentInventoryItem, transform.position);
            }
        }

        public void Setup(OrbitCamera orbitCamera, Transform worldObjectsRoot, WorldStreamer worldStreamer)
        {
            this.orbitCamera = orbitCamera;
            this.worldObjectsRoot = worldObjectsRoot;
            this.worldStreamer = worldStreamer;
            this.world = worldStreamer.World;

            inputController.Setup(orbitCamera);
            inventory = new Inventory(
                new InventoryState());

            isSetup = true;
        }
    }
}