using System;
using UniRx;
using UnityEngine;

namespace MageSimulator.Controller.Wiimote.MotionDetector
{
    public class MotionDetector : MonoBehaviour
    {
        public WiimoteDevice WiimoteDevice { get; private set; }
        public IObservable<Unit> OnDetect => _onDetect;
        private readonly Subject<Unit> _onDetect = new Subject<Unit>();

        public void SetDevice(string deviceId)
        {
            WiimoteDevice = WiimoteExtension.GetDevice(deviceId);
        }

        public void SetCurrentDevice()
        {
            WiimoteDevice = WiimoteDevice.Current;
        }

        protected void Detect()
        {
            _onDetect.OnNext(Unit.Default);
        }
    }
}