using System;
using System.Collections.Generic;
using Profighter.Client.Camera;
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

        private IInteractableEntity currentInteractableEntity;
        private Transform worldObjectsRoot;
        private WorldStreamer worldStreamer;

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
                var isInteractable = TryGetInteractable(raycastHits[0].collider, out var interactable);

                if (isInteractable)
                {
                    currentInteractableEntity = interactable;
                }
                else
                {
                    currentInteractableEntity = null;
                }
            }
            else
            {
                currentInteractableEntity = null;
            }
        }

        private bool TryGetInteractable(Collider collider, out IInteractableEntity interactableEntity)
        {
            foreach (var interactableObject in worldStreamer.InteractableObjects)
            {
                if (interactableObject.InteractableEntity == null)
                {
                    continue;
                }

                var interactableEntityCollider = interactableObject.InteractableEntity.Collider;
                if (interactableEntityCollider != null && interactableEntityCollider == collider)
                {
                    interactableEntity = interactableObject.InteractableEntity;
                    return true;
                }
            }

            interactableEntity = null;
            return false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && currentInteractableEntity != null)
            {
                worldStreamer.RemoveSceneObject(currentInteractableEntity);
                inventory.Add(currentInteractableEntity);
                characterController.IgnoredColliders.Add(currentInteractableEntity.Collider);
                currentInteractableEntity.Transform.parent = holdObjectRoot;
                currentInteractableEntity.Transform.position = holdObjectRoot.position;
                currentInteractableEntity.Transform.rotation = holdObjectRoot.rotation;
            }
            else if (Input.GetKeyDown(KeyCode.Y) && inventory.GetItem() != null)
            {
                var interactable = inventory.GetItem();
                characterController.IgnoredColliders.Remove(interactable.Collider);
                interactable.Transform.parent = worldObjectsRoot;
                inventory.RemoveItem();
                worldStreamer.AddSceneObject(interactable, transform.position);
            }
        }

        public void Setup(OrbitCamera orbitCamera, Transform worldObjectsRoot, WorldStreamer worldStreamer)
        {
            this.orbitCamera = orbitCamera;
            this.worldObjectsRoot = worldObjectsRoot;
            this.worldStreamer = worldStreamer;

            inputController.Setup(orbitCamera);
            inventory = new Inventory();

            isSetup = true;
        }
    }
}