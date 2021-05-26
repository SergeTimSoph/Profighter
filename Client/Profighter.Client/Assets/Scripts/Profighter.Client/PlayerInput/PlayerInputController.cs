using KinematicCharacterController;
using Profighter.Client.Camera;
using Profighter.Client.Character;
using UnityEngine;
using CharacterController = Profighter.Client.Character.CharacterController;

namespace Profighter.Client.PlayerInput
{
    public class PlayerInputController : MonoBehaviour
    {
        public CharacterController Character;
        public OrbitCamera CharacterCamera;

        private const string RotationXInput = "RotationX";
        private const string RotationYInput = "RotationY";
        private const string HorizontalInput = "MovementHorizontal";
        private const string VerticalInput = "MovementVertical";

        private void Start()
        {
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = CrossPlatformInputManager.GetAxisRaw(RotationYInput);
            float mouseLookAxisRight = CrossPlatformInputManager.GetAxisRaw(RotationXInput);

            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            CharacterCamera.UpdateWithInput(Time.deltaTime, lookInputVector);
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

            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.JumpDown = UnityEngine.Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = UnityEngine.Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = UnityEngine.Input.GetKeyUp(KeyCode.C);

            Character.SetInputs(ref characterInputs);
        }
    }
}