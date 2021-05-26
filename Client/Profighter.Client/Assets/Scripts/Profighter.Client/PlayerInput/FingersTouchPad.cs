using System;
using DigitalRubyShared;
using UnityEngine;

namespace Profighter.Client.PlayerInput
{
    public class FingersTouchPad : MonoBehaviour
    {
        [SerializeField]
        private GameObject panView;

        [SerializeField]
        private float panGestureThresholdUnits = 0.1f;

        [SerializeField]
        private string horizontalAxisName = "RotationX";
        [SerializeField]
        private string verticalAxisName = "RotationY";

        private PanGestureRecognizer panGesture;
        private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;
        private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

        private void OnEnable()
        {
            CreateAndAddGestureRecognizer();
            CreateAndAddVirtualAxes();
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
            horizontalVirtualAxis.Update(0.0f);
            verticalVirtualAxis.Update(0.0f);
        }

        private void HandlePanExecutingGesture(GestureRecognizer panGesture)
        {
            var unitsX = DeviceInfo.PixelsToUnits(panGesture.DeltaX);
            var unitsY = DeviceInfo.PixelsToUnits(panGesture.DeltaY);

            horizontalVirtualAxis.Update(unitsX);
            verticalVirtualAxis.Update(unitsY);
        }

        private void HandlePanEndedOrFailedGesture()
        {
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
