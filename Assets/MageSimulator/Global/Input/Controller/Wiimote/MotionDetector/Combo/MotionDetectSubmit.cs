using MageSimulator.Combo.Components;
using UniRx;
using UnityEngine;

namespace MageSimulator.Global.Input.Controller.Wiimote.MotionDetector.Combo
{
    [RequireComponent(typeof(MotionDetector))]
    public class MotionDetectSubmit : Submit
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
            args.name = _detector.WiimoteDevice.description.product;
            Publish();
        }
    }
}