using System.Linq;
using UnityEngine.InputSystem;

namespace MageSimulator.Controller.Wiimote
{
    public static class WiimoteExtension
    {
        public static WiimoteDevice GetDevice(string deviceId)
        {
            return InputSystem.devices.First(x => x.description.product == deviceId) as WiimoteDevice;
        }

        public static string GetDeviceId(this WiimoteDevice device)
        {
            return device.description.product;
        }
    }
}