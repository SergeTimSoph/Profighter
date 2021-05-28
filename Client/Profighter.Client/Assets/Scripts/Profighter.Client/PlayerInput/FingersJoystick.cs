using System;
using DigitalRubyShared;
using UnityEngine;

namespace Profighter.Client.PlayerInput
{
    public class FingersJoystick : MonoBehaviour
    {
        [SerializeField]
        private GameObject panView;
        [SerializeField]
        private RectTransform joystickBackgroundRect;
        [SerializeField]
        private RectTransform joystickForegroundRect;
        [SerializeField]
        private float joystickForegroundRectDiameterMultiplier = 0.3f;

        [SerializeField]
        private float panUnitsForMaxMove = 1f;
        [SerializeField]
        private float panGestureThresholdUnits = 0.1f;

        [SerializeField]
        private string horizontalAxisName = "Horizontal";
        [SerializeField]
        private string verticalAxisName = "Vertical";

        private Vector2 panBeganPosition;
        private Vector2 initialJoystickBackgroundPosition;
        private Vector2 initialJoystickForegroundPosition;

        private PanGestureRecognizer panGesture;
        private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;
        private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

        private void OnEnable()
        {
            CreateAndAddGestureRecognizer();
            CreateAndAddVirtualAxes();
        }

        private void Start()
        {
            var joystickBackgroundDiameter = DeviceInfo.UnitsToPixels(panUnitsForMaxMove) * 2;
            joystickBackgroundRect.sizeDelta = new Vector2(joystickBackgroundDiameter, joystickBackgroundDiameter);
            var joystickForegroundDiameter = joystickBackgroundDiameter * joystickForegroundRectDiameterMultiplier;
            joystickForegroundRect.sizeDelta = new Vector2(joystickForegroundDiameter, joystickForegroundDiameter);

            var joystickBackgroundPosition = joystickBackgroundRect.position;
            panBeganPosition = joystickBackgroundPosition;

            initialJoystickBackgroundPosition = joystickBackgroundPosition;
            initialJoystickForegroundPosition = joystickForegroundRect.position;
        }

        private void CreateAndAddGestureRecognizer()
        {
            panGesture = new PanGestureRecognizer
            {
                PlatformSpecificView = panView,
                ThresholdUnits = panGestureThresholdUnits
            };
            panGesture.StateUpdated += PanGestureUpdatedHandler;
            FingersScript.Instance.AddGesture(panGesture);
        }

        private void PanGestureUpdatedHandler(GestureRecognizer panGesture)
        {
            switch (panGesture.State)
            {
                case GestureRecognizerState.Began:
                    HandlePanBeganGesture(panGesture);
                    break;
                case GestureRecognizerState.Executing:
                    HandlePanExecutingGesture(panGesture);
                    break;
                case GestureRecognizerState.Ended:
                case GestureRecognizerState.Failed:
                    HandlePanEndedOrFailedGesture();
                    break;
                case GestureRecognizerState.Possible:
                case GestureRecognizerState.EndPending:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePanBeganGesture(GestureRecognizer panGesture)
        {
            var joystickBackgroundPosition = joystickBackgroundRect.position;
            joystickBackgroundPosition = new Vector3(panGesture.StartFocusX, panGesture.StartFocusY, joystickBackgroundPosition.z);
            joystickBackgroundRect.position = joystickBackgroundPosition;
            panBeganPosition = joystickBackgroundPosition;
        }

        private void HandlePanExecutingGesture(GestureRecognizer panGesture)
        {
            var unitsX = DeviceInfo.PixelsToUnits(panGesture.DistanceX);
            var unitsY = DeviceInfo.PixelsToUnits(panGesture.DistanceY);
            var panX = Mathf.Sign(unitsX) * Mathf.Lerp(0.0f, 1.0f, Mathf.Abs(unitsX) / panUnitsForMaxMove);
            var panY = Mathf.Sign(unitsY) * Mathf.Lerp(0.0f, 1.0f, Mathf.Abs(unitsY) / panUnitsForMaxMove);

            horizontalVirtualAxis.Update(panX);
            verticalVirtualAxis.Update(panY);

            var pan = new Vector2(panGesture.DistanceX, panGesture.DistanceY);
            var panMagnitude = pan.magnitude;

            if (panMagnitude > DeviceInfo.UnitsToPixels(panUnitsForMaxMove))
            {
                pan = pan.normalized * DeviceInfo.UnitsToPixels(panUnitsForMaxMove);
            }

            joystickForegroundRect.position = panBeganPosition + pan;
        }

        private void HandlePanEndedOrFailedGesture()
        {
            joystickBackgroundRect.position = initialJoystickBackgroundPosition;
            joystickForegroundRect.position = initialJoystickForegroundPosition;

            horizontalVirtualAxis.Update(0.0f);
            verticalVirtualAxis.Update(0.0f);
        }

        private void CreateAndAddVirtualAxes()
        {
            horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(horizontalVirtualAxis);

            verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(verticalVirtualAxis);
        }

        private void OnDisable()
        {
            RemoveGestureRecognizer();
            RemoveVirtualAxes();
        }

        private void RemoveGestureRecognizer()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(panGesture);
            }
        }

        private void RemoveVirtualAxes()
        {
            horizontalVirtualAxis.Remove();
            verticalVirtualAxis.Remove();
        }
    }
}
