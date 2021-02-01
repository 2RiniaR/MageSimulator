using MageSimulator.Controller.Wiimote.MotionDetector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MageSimulator.Controller.Wiimote.Calibrator.Scripts
{
    public class CalibrateStepPage : MonoBehaviour
    {
        public RestMotionDetector motionDetector;
        public Image gaugeFill;

        private void Start()
        {
            motionDetector.continueTime.Subscribe(x =>
            {
                gaugeFill.fillAmount = Mathf.Clamp01(x / motionDetector.requireTime);
            }).AddTo(this);
        }
    }
}