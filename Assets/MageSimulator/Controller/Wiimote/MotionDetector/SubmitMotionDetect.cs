using MageSimulator.BrowserUI;
using MageSimulator.BrowserUI.Events;
using UniRx;
using UnityEngine;

namespace MageSimulator.Controller.Wiimote.MotionDetector
{
    [RequireComponent(typeof(MotionDetector))]
    public class SubmitMotionDetect : BrowserComponent
    {
        private MotionDetector _detector;

        protected override void Start()
        {
            _detector = GetComponent<MotionDetector>();
            _detector.OnDetect.Subscribe(_ => OnDetect()).AddTo(this);
            base.Start();
        }

        private void OnDetect()
        {
            PublishEvent(new SubmitEvent {name = _detector.WiimoteDevice.description.product});
        }
    }
}