using KinematicCharacterController;
using Profighter.Client.Camera;
using Profighter.Client.Character;
using UnityEngine;
using CharacterController = Profighter.Client.Character.CharacterController;

namespace Profighter.Client.PlayerInput
{
    public class CharacterInputController : MonoBehaviour
    {
        [SerializeField]
        private CharacterController characterController;

        private OrbitCamera characterCamera;
        private bool isSetup;

        private const string RotationXInput = "RotationX";
        private const string RotationYInput = "RotationY";
        private const string HorizontalInput = "MovementHorizontal";
        private const string VerticalInput = "MovementVertical";

        private void Update()
        {
            if (!isSetup)
            {
                return;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            if (!isSetup)
            {
                return;
            }

            if (characterCamera.RotateWithPhysicsMover && characterController.Motor.AttachedRigidbody != null)
            {
                characterCamera.PlanarDirection = characterController.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * characterCamera.PlanarDirection;
                characterCamera.PlanarDirection = Vector3.ProjectOnPlane(characterCamera.PlanarDirection, characterController.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        public void Setup(OrbitCamera camera)
        {
            characterCamera = camera;

            characterCamera.SetFollowTransform(characterController.CameraFollowPoint);

            characterCamera.IgnoredColliders.Clear();
            characterCamera.IgnoredColliders.AddRange(characterController.GetComponentsInChildren<Collider>());

            isSetup = true;
        }

        private void HandleCameraInput()
        {
            float mouseLookAxisUp = MultiPlatformInputManager.GetAxisRaw(RotationYInput);
            float mouseLookAxisRight = MultiPlatformInputManager.GetAxisRaw(RotationXInput);

            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            characterCamera.UpdateWithInput(Time.deltaTime, lookInputVector);
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            #if UNITY_EDITOR
            characterInputs.MoveAxisForward = UnityEngine.Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = UnityEngine.Input.GetAxisRaw(HorizontalInput);
            #else
            characterInputs.MoveAxisForward = CrossPlatformInputManager.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = CrossPlatformInputManager.GetAxisRaw(HorizontalInput);
            #endif

            characterInputs.CameraRotation = characterCamera.Transform.rotation;

            characterController.SetInputs(ref characterInputs);
        }
    }
}