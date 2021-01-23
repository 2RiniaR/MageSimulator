using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using WiimoteApi;

namespace MageSimulator.Wiimote.InputSystem.Scripts
{
    using InputSystem = UnityEngine.InputSystem.InputSystem;

    [ExecuteInEditMode]
    public class WiimoteDeviceSupport : MonoBehaviour
    {
        private const string InterfaceName = "WiimoteApi";
        public float detectRemovedDeviceInterval = 1f;
        public bool findOnAwake = true;
        private float _detectRemovedDeviceTime;
        private readonly List<string> _wiimoteHids = new List<string>();

        protected void Awake()
        {
            InputSystem.RegisterLayout<WiimoteDevice>(matches: new InputDeviceMatcher().WithInterface(InterfaceName));
        }

        private void Start()
        {
            _detectRemovedDeviceTime = detectRemovedDeviceInterval;
            if (findOnAwake) Find();
        }

        private void Update()
        {
            _detectRemovedDeviceTime -= Time.deltaTime;
            if (_detectRemovedDeviceTime <= 0f)
            {
                _detectRemovedDeviceTime = detectRemovedDeviceInterval;
                DetectRemovedDevice();
            }
        }

        private void OnDisable()
        {
            foreach (var device in InputSystem.devices.Where(x => x.description.interfaceName == InterfaceName))
                InputSystem.RemoveDevice(device);
        }

        public void Find()
        {
            WiimoteManager.FindWiimotes();
            Debug.Log(WiimoteManager.Wiimotes.Count + " wiimotes found.");
            var added = WiimoteManager.Wiimotes.Select(w => w.hidapi_path).Except(_wiimoteHids);
            foreach (var id in added)
            {
                var wiimote = WiimoteManager.Wiimotes.Find(x => x.hidapi_path == id);
                if (wiimote == null) break;

                Debug.Log("Wiimote connected.");
                wiimote.SendPlayerLED(true, false, false, true);
                wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
                if (wiimote.wmp_attached || wiimote.Type == WiimoteType.PROCONTROLLER)
                    wiimote.RequestIdentifyWiiMotionPlus();
                if (wiimote.ActivateWiiMotionPlus())
                    Debug.Log("Wii motion plus has been activated.");

                InputSystem.AddDevice(new InputDeviceDescription
                {
                    interfaceName = InterfaceName,
                    product = id
                });
            }
        }

        private void DetectRemovedDevice()
        {
            var removed = _wiimoteHids.Except(WiimoteManager.Wiimotes.Select(w => w.hidapi_path));
            foreach (var id in removed)
            {
                var device = InputSystem.devices.FirstOrDefault(
                    x => x.description == new InputDeviceDescription
                    {
                        interfaceName = InterfaceName,
                        product = id
                    });

                if (device != null)
                    InputSystem.RemoveDevice(device);
            }
        }
    }
}