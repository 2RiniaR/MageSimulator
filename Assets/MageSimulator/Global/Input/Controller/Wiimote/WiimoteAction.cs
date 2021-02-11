using UnityEngine;

namespace MageSimulator.Global.Input.Controller.Wiimote
{
    public class WiimoteAction : MonoBehaviour
    {
        public WiimoteDevice WiimoteDevice { get; private set; }
        public bool activeCurrentDeviceOnStart;

        protected virtual void Start()
        {
            if (activeCurrentDeviceOnStart)
                SetCurrentDevice();
        }

        public void SetDevice(string deviceId)
        {
            WiimoteDevice = WiimoteExtension.GetDevice(deviceId);
        }

        public void SetCurrentDevice()
        {
            WiimoteDevice = WiimoteDevice.All.Count > 0 ? WiimoteDevice.All[0] : null;
        }
    }
}