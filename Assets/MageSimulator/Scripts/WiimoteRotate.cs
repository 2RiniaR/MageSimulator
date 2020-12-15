using System.Collections;
using UnityEngine;
using WiimoteApi;

namespace MageSimulator.Scripts
{
    public class WiimoteRotate : MonoBehaviour
    {
        private WiimoteApi.Wiimote _wiimote;

        private void Update()
        {
            if (_wiimote == null)
                return;

            while (true)
            {
                var ret = _wiimote.ReadWiimoteData();
                if (ret > 0 && _wiimote.current_ext == ExtensionController.MOTIONPLUS)
                {
                    var offset = new Vector3(-_wiimote.MotionPlus.PitchSpeed, _wiimote.MotionPlus.YawSpeed,
                        _wiimote.MotionPlus.RollSpeed) / 95f;
                    transform.Rotate(offset, Space.Self);
                }
                if (ret <= 0) break;
            }
        }

        private static Vector3 GetAccelVector(WiimoteApi.Wiimote wiimote)
        {
            var accel = wiimote.Accel.GetCalibratedAccelData();
            return new Vector3(accel[0], -accel[2], -accel[1]).normalized;
        }
    }
}
