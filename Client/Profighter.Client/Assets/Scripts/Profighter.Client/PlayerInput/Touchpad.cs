using Profighter.Client.UI;
using UnityEngine;

namespace Profighter.Client.PlayerInput
{
    public class Touchpad : MonoBehaviour
    {
        [SerializeField]
        private string horizontalAxisName = "RotationX";
        [SerializeField]
        private string verticalAxisName = "RotationY";

        [SerializeField]
        private Rect touchRect = new Rect(0.5f, 0.0f, 0.5f, 1f);

        private MultiPlatformInputManager.VirtualAxis horizontalVirtualAxis;
        private MultiPlatformInputManager.VirtualAxis verticalVirtualAxis;

        private bool hasPointInArea;

        private Vector2 prevPosition;

        private void OnEnable()
        {
            horizontalVirtualAxis = new MultiPlatformInputManager.VirtualAxis(horizontalAxisName);
            MultiPlatformInputManager.RegisterVirtualAxis(horizontalVirtualAxis);

            verticalVirtualAxis = new MultiPlatformInputManager.VirtualAxis(verticalAxisName);
            MultiPlatformInputManager.RegisterVirtualAxis(verticalVirtualAxis);
        }

        private void OnDisable()
        {
            horizontalVirtualAxis.Remove();
            verticalVirtualAxis.Remove();
        }

        private void Update()
        {
#if UNITY_EDITOR
            ProcessMouseInput();
#else
            ProcessTouchInput();
#endif
        }

#if UNITY_EDITOR
        private void ProcessMouseInput()
        {
            if (Input.GetMouseButton(0))
            {
                HandlePointerInArea(Input.mousePosition);
            }
            else
            {
                hasPointInArea = false;
                UpdateVirtualAxes(Vector2.zero);
            }
        }
#else
        private void ProcessTouchInput()
        {
            var index = GetActiveTouchIndex();

            if (index >= 0)
            {
                HandlePointerInArea(Input.GetTouch(index).position);
            }
            else
            {
                hasPointInArea = false;
                UpdateVirtualAxes(Vector2.zero);
            }
        }

#endif

        private void HandlePointerInArea(Vector2 pointerPosition)
        {
            if (!hasPointInArea)
            {
                hasPointInArea = true;
                prevPosition = pointerPosition;
            }

            var delta = pointerPosition - prevPosition;

            UpdateVirtualAxes(delta);

            prevPosition = pointerPosition;
        }

        private int GetActiveTouchIndex()
        {
            for (var index = 0; index < Input.touchCount; index++)
            {
                var touch = Input.GetTouch(index);

                if (IsPositionInRect(touch.position))
                {
                    return index;
                }
            }

            return -1;
        }

        private bool IsPositionInRect(Vector2 position)
        {
            var normalizedPosition = new Vector2(position.x / Screen.width, position.y / Screen.height);

            return touchRect.Contains(normalizedPosition);
        }

        private void UpdateVirtualAxes(Vector2 value)
        {
            var inchesX = DeviceScreenInfo.PixelsToInches(value.x);
            var inchesY = DeviceScreenInfo.PixelsToInches(value.y);

            horizontalVirtualAxis.Update(inchesX);
            verticalVirtualAxis.Update(inchesY);
        }
    }
}