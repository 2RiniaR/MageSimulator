/*

using System;
using System.Collections;
using MageSimulator.Utils;
using MageSimulator.Utils.Scripts;
using MageSimulator.Wiimote.Calibrator.Scripts;
using UnityEngine;
using WiimoteApi;

namespace MageSimulator.Wiimote.Scripts
{
    public class WiimoteActivator : SingletonMonoBehaviour<WiimoteActivator>
    {
        public WiimoteApi.Wiimote Wiimote { get; private set; }
        private readonly StayDetector _stayDetector = new StayDetector(10);
        private readonly WiimoteRotation _wiimoteRotation = new WiimoteRotation();
        public float AccelMagnitudeAverage => _stayDetector.AccelMagnitudeAverage;
        public Quaternion Rotation => _wiimoteRotation.Rotation;

        private void Start()
        {
            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            while (!WiimoteManager.HasWiimote())
            {
                WiimoteManager.FindWiimotes();
                yield return null;
            }
            Wiimote = WiimoteManager.Wiimotes[0];
            Debug.Log("Wiimote connected.");
            Wiimote.SendPlayerLED(true, false, false, true);
            Wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
            while (!(Wiimote.wmp_attached || Wiimote.Type == WiimoteType.PROCONTROLLER))
            {
                Wiimote.RequestIdentifyWiiMotionPlus();
                yield return null;
            }
            Wiimote.ActivateWiiMotionPlus();
            Debug.Log("Wii motion plus has been activated.");
        }

        private void Update()
        {
            if (Wiimote == null) return;
            while (Wiimote.ReadWiimoteData() > 0)
            {
                if (Wiimote.current_ext == ExtensionController.MOTIONPLUS)
                    _wiimoteRotation.Rotate();
            }

            _stayDetector.Update();
            _wiimoteRotation.Update();
        }

        public Vector3 GetAccelVector()
        {
            var accel = Wiimote.Accel.GetCalibratedAccelData();
            return new Vector3(accel[0], -accel[2], -accel[1]).normalized;
        }

        public Vector3 GetRollVector()
        {
            if (Wiimote.MotionPlus == null)
                return Vector3.zero;
            return new Vector3(Wiimote.MotionPlus.PitchSpeed, Wiimote.MotionPlus.YawSpeed,
                Wiimote.MotionPlus.RollSpeed) / 95f;
        }

        public void ResetRotation() => _wiimoteRotation.ResetRotation();
    }
}

*/
