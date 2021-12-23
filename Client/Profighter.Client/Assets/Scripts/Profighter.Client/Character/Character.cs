using System;
using System.Collections.Generic;
using Profighter.Client.Camera;
using Profighter.Client.PlayerInput;
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
        private IDictionary<Collider, IInteractable> interactableObjects;

        private IInteractable currentInteractable;
        private Transform worldObjectsRoot;

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
                var isInteractable = interactableObjects.TryGetValue(raycastHits[0].collider, out var interactable);

                if (isInteractable)
                {
                    currentInteractable = interactable;
                }
                else
                {
                    currentInteractable = null;
                }
            }
            else
            {
                currentInteractable = null;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && currentInteractable != null)
            {
                inventory.Add(currentInteractable);
                characterController.IgnoredColliders.Add(currentInteractable.Collider);
                currentInteractable.Transform.parent = holdObjectRoot;
                currentInteractable.Transform.position = holdObjectRoot.position;
                currentInteractable.Transform.rotation = holdObjectRoot.rotation;
            }
            else if (Input.GetKeyDown(KeyCode.Y) && inventory.GetItem() != null)
            {
                var interactable = inventory.GetItem();
                characterController.IgnoredColliders.Remove(interactable.Collider);
                interactable.Transform.parent = worldObjectsRoot;
                inventory.RemoveItem();
            }
        }

        public void Setup(OrbitCamera orbitCamera, IDictionary<Collider, IInteractable> interactableObjects, Transform worldObjectsRoot)
        {
            this.orbitCamera = orbitCamera;
            this.interactableObjects = interactableObjects;
            this.worldObjectsRoot = worldObjectsRoot;

            inputController.Setup(orbitCamera);
            inventory = new Inventory();

            isSetup = true;
        }
    }
}