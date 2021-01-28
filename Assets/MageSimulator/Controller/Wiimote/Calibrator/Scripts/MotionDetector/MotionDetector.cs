using System.Linq;
using MageSimulator.BrowserUI;
using MageSimulator.Controller.Wiimote.InputSystem.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace MageSimulator.Controller.Wiimote.Calibrator.Scripts.MotionDetector
{
    public abstract class MotionDetector : BrowserComponent
    {
        public UnityEvent<bool> onConditionChanged;
        protected WiimoteDevice WiimoteDevice;
        private bool _previousInCondition;

        public void SetDevice(string deviceId)
        {
            WiimoteDevice =
                UnityEngine.InputSystem.InputSystem.devices.First(x => x.description.product == deviceId) as
                    WiimoteDevice;
        }

        private void Update()
        {
            var inCondition = WiimoteDevice != null && InCondition();
            if (_previousInCondition ^ inCondition)
            {
                onConditionChanged.Invoke(inCondition);
                _previousInCondition = inCondition;
            }
        }

        public abstract bool InCondition();
    }
}