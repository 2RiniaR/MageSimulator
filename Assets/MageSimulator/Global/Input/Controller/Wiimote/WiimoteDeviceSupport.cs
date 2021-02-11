using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using WiimoteApi;

namespace MageSimulator.Global.Input.Controller.Wiimote
{

    [ExecuteInEditMode]
    public class WiimoteDeviceSupport : MonoBehaviour
    {
        private const string InterfaceName = "WiimoteApi";
        public float detectRemovedDeviceInterval = 1f;
        public bool findOnAwake = true;
        private float _detectRemovedDeviceTime;
        private readonly List<string> _wiimoteHids = new List<string>();
        public InputAction rotationResetAction;
        public InputAction rotationCalibrateAction;

        private static readonly int[,] AccelCalibration = {
            { 502, 508, 604 },
            { 501, 604, 501 },
            { 608, 504, 508 }
        };

        protected void Awake()
        {
            InputSystem.RegisterLayout<WiimoteDevice>(matches: new InputDeviceMatcher().WithInterface(InterfaceName));
        }

        private void OnEnable()
        {
            rotationResetAction.Enable();
            rotationCalibrateAction.Enable();
        }

        private void OnDestroy()
        {
            rotationResetAction.Disable();
            rotationCalibrateAction.Disable();
        }

        private void Start()
        {
            Observable.FromEvent<InputAction.CallbackContext>(h => rotationResetAction.performed += h,
                    h => rotationResetAction.performed -= h)
                .Subscribe(_ =>
                {
                    var device = InputSystem.GetDevice<WiimoteDevice>();
                    if (device == null) return;
                    device.ResetRotation();
                })
                .AddTo(this);

            Observable.FromEvent<InputAction.CallbackContext>(h => rotationCalibrateAction.performed += h,
                    h => rotationCalibrateAction.performed -= h)
                .Subscribe()
                .AddTo(this);

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

                var device = InputSystem.AddDevice(new InputDeviceDescription
                {
                    interfaceName = InterfaceName,
                    product = id
                }) as WiimoteDevice;

                if (device != null)
                    device.Wiimote.Accel.accel_calib = AccelCalibration;
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