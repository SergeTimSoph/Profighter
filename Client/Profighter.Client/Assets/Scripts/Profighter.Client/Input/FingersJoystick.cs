using DigitalRubyShared;
using UnityEngine;

namespace Profighter.Client.Input
{
    public class FingersJoystick : MonoBehaviour
    {
        [SerializeField]
        private GameObject panView;
        [SerializeField]
        private float panGestureThresholdUnits = 0.1f;

        private PanGestureRecognizer panGesture;

        private void OnEnable()
        {
            panGesture = new PanGestureRecognizer()
            {
                PlatformSpecificView = panView,
                ThresholdUnits = panGestureThresholdUnits
            };
            panGesture.StateUpdated += Panned;

            FingersScript.Instance.AddGesture(panGesture);
        }

        private void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(panGesture);
            }
        }

        private void Panned(GestureRecognizer gesture)
        {

        }
    }
}
