using System;
using System.Linq;
using UnityEngine;

namespace MyKinematicCCExamples._1
{
    public class MyPlayer : MonoBehaviour
    {
        [SerializeField]
        private MyCharacterCamera orbitCamera;

        [SerializeField]
        private Transform cameraFollowPoint;

        [SerializeField]
        private MyCharacterController characterController;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private Vector3 lookInputVector = Vector3.zero;

        private void Start()
        {
            orbitCamera.SetFollowTransform(cameraFollowPoint);
            orbitCamera.IgnoredColliders = characterController.GetComponentsInChildren<Collider>().ToList();
        }

        private void Update()
        {
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            var lookAxisUp = Input.GetAxisRaw(MouseXInput);
            var lookAxisRight = Input.GetAxisRaw(MouseYInput);
            lookInputVector = new Vector3(lookAxisRight, lookAxisUp, 0f);

            orbitCamera.UpdateWithInput(Time.deltaTime, lookInputVector);
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = orbitCamera.transform.rotation;

            characterController.SetInputs(ref characterInputs);
        }
    }
}
