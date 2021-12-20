using System;
using Profighter.Client.Camera;
using Profighter.Client.PlayerInput;
using UnityEngine;

namespace Profighter.Client.Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private CharacterInputController inputController;

        [SerializeField]
        private LayerMask raycastMask;

        private bool isSetup;
        private Inventory inventory;
        private RaycastHit[] raycastHits = new RaycastHit[5];
        private OrbitCamera orbitCamera;

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

            Debug.LogWarning($"Hits count: {hits}");
            for (int i = 0; i < hits; i++)
            {

            }
        }

        public void Setup(OrbitCamera orbitCamera)
        {
            this.orbitCamera = orbitCamera;
            inputController.Setup(orbitCamera);
            inventory = new Inventory();

            isSetup = true;
        }
    }
}