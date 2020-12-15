using System.Collections;
using UnityEngine;
using WiimoteApi;

namespace MageSimulator.Wiimote.Scripts
{
    public class WiimoteActivator : MonoBehaviour
    {
        public WiimoteApi.Wiimote Wiimote { get; private set; }

        private IEnumerator Setup()
        {
            while (!WiimoteManager.HasWiimote())
            {
                WiimoteManager.FindWiimotes();
                yield return null;
            }
            Wiimote = WiimoteManager.Wiimotes[0];
            Wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
            while (!(Wiimote.wmp_attached || Wiimote.Type == WiimoteType.PROCONTROLLER))
            {
                Wiimote.RequestIdentifyWiiMotionPlus();
                yield return null;
            }
            Wiimote.ActivateWiiMotionPlus();
            Wiimote.MotionPlus.SetZeroValues();
        }

        private void Update()
        {
            if (Wiimote == null)
                return;

            while (true)
            {
                var ret = Wiimote.ReadWiimoteData();
                if (ret > 0 && Wiimote.current_ext == ExtensionController.MOTIONPLUS)
                {
                    var offset = new Vector3(-Wiimote.MotionPlus.PitchSpeed, Wiimote.MotionPlus.YawSpeed,
                        Wiimote.MotionPlus.RollSpeed) / 95f;
                    transform.Rotate(offset, Space.Self);
                }
                if (ret <= 0) break;
            }
        }

        public Vector3 GetAccelVector()
        {
            var accel = Wiimote.Accel.GetCalibratedAccelData();
            return new Vector3(accel[0], -accel[2], -accel[1]).normalized;
        }

        public void CalibrateAccelerometer(AccelCalibrationStep step) {
            Wiimote.Accel.CalibrateAccel(step);
        }
    }
}