using Profighter.Client.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;

namespace Profighter.Client.PlayerInput
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private RectTransform touchAreaRect;
        [SerializeField]
        private RectTransform joystickBackgroundRect;
        [SerializeField]
        private RectTransform joystickForegroundRect;
        [SerializeField]
        private float joystickForegroundRectDiameterFactor = 0.3f;

        [SerializeField]
        private float movementRangeInInches = 1f;

        [SerializeField]
        private string horizontalAxisName = "MovementHorizontal";
        [SerializeField]
        private string verticalAxisName = "MovementVertical";

        private Vector2 dragStartPosition;

        private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;
        private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

        private Vector3 initialJoystickForegroundPosition;
        private Vector3 initialJoystickBackgroundPosition;

        private void OnEnable()
        {
            horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(horizontalVirtualAxis);

            verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(verticalVirtualAxis);
        }

        private void Start()
        {
            var joystickBackgroundDiameter = DeviceScreenInfo.InchesToPixels(movementRangeInInches) * 2;
            joystickBackgroundRect.sizeDelta = new Vector2(joystickBackgroundDiameter, joystickBackgroundDiameter);
            var joystickForegroundDiameter = joystickBackgroundDiameter * joystickForegroundRectDiameterFactor;
            joystickForegroundRect.sizeDelta = new Vector2(joystickForegroundDiameter, joystickForegroundDiameter);

            var joystickBackgroundPosition = joystickBackgroundRect.position;
            dragStartPosition = touchAreaRect.InverseTransformPoint(joystickBackgroundPosition);

            initialJoystickForegroundPosition = joystickForegroundRect.localPosition;
            initialJoystickBackgroundPosition = joystickBackgroundPosition;
        }

        private void OnDisable()
        {
            horizontalVirtualAxis.Remove();
            verticalVirtualAxis.Remove();
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(touchAreaRect, data.position, data.pressEventCamera, out var position))
            {
                return;
            }

            joystickBackgroundRect.position = touchAreaRect.TransformPoint(position);
            dragStartPosition = touchAreaRect.InverseTransformPoint(joystickBackgroundRect.position);
        }

        public void OnDrag(PointerEventData data)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(touchAreaRect, data.position, data.pressEventCamera, out var dragPosition))
            {
                return;
            }

            var dragOffset = dragPosition - dragStartPosition;

            var sqrMagnitude = dragOffset.sqrMagnitude;
            if (sqrMagnitude > DeviceScreenInfo.InchesToPixels(movementRangeInInches) * DeviceScreenInfo.InchesToPixels(movementRangeInInches))
            {
                dragOffset = dragOffset.normalized * DeviceScreenInfo.InchesToPixels(movementRangeInInches);
            }

            var dragVector = new Vector3(dragOffset.x, dragOffset.y);
            joystickForegroundRect.localPosition = dragVector;

            UpdateVirtualAxes(dragVector);
        }

        public void OnPointerUp(PointerEventData data)
        {
            joystickBackgroundRect.position = initialJoystickBackgroundPosition;
            joystickForegroundRect.localPosition = initialJoystickForegroundPosition;

            UpdateVirtualAxes(Vector2.zero);
        }

        private void UpdateVirtualAxes(Vector2 value)
        {
            var inchesX = DeviceScreenInfo.PixelsToInches(value.x);
            var inchesY = DeviceScreenInfo.PixelsToInches(value.y);

            var inchesRelativeToRangeX = Mathf.Sign(inchesX) * Mathf.Lerp(0.0f, 1.0f, Mathf.Abs(inchesX) / movementRangeInInches);
            var inchesRelativeToRangeY = Mathf.Sign(inchesY) * Mathf.Lerp(0.0f, 1.0f, Mathf.Abs(inchesY) / movementRangeInInches);

            horizontalVirtualAxis.Update(inchesRelativeToRangeX);
            verticalVirtualAxis.Update(inchesRelativeToRangeY);
        }
    }
}