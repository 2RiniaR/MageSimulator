/*

using UnityEngine;

namespace MageSimulator.Wiimote.Scripts
{
    public class WiimoteRotation
    {
        private const float StillThreshold = 0.005f;
        private const float MinAutoFixInterval = 0.5f;
        private const float FixRotationSpeed = 0.1f;
        public Quaternion Rotation { get; private set; } = Quaternion.identity;
        private Quaternion _fixedRotation = Quaternion.identity;
        private float _elapsedTimeFromLastAutoFix = 0f;

        public void Rotate()
        {
            var role = WiimoteActivator.Instance.GetRollVector();
            var quaternion = Quaternion.Euler(role.x, role.y, role.z);
            Rotation *= quaternion;
        }

        public void Update()
        {
            _elapsedTimeFromLastAutoFix += Time.deltaTime;
            // if (_elapsedTimeFromLastAutoFix >= MinAutoFixInterval &&
            //     WiimoteActivator.Instance.AccelMagnitudeAverage < StillThreshold)
            //     FixRotation();
        }

        public void FixRotation()
        {
            var current = Rotation.eulerAngles;
            var accel = WiimoteActivator.Instance.GetAccelVector().normalized;

            var xz = new Vector2(accel.x, accel.y);
            if (xz.magnitude > 0)
            {
                var zAngle = Vector2.SignedAngle(Vector2.down, xz.normalized);
                current.z = zAngle;
            }

            var yz = new Vector2(accel.y, accel.z);
            if (yz.magnitude > 0)
            {
                var xAngle = -Mathf.Asin(yz.normalized.y) * Mathf.Rad2Deg;
                current.x = xAngle;
                // if (yz.normalized.x > 0 && current.y < 180f || yz.normalized.x < 0 && current.y > 180f)
                //     current.y = (current.y + 180f) % 360f;
            }

            Debug.Log("Fix rotated: " + current);
            Rotation = Quaternion.Euler(current);
            _elapsedTimeFromLastAutoFix = 0f;
        }

        public void ResetRotation()
        {
            Rotation = Quaternion.identity;
            WiimoteActivator.Instance.Wiimote.MotionPlus?.SetZeroValues();
        }
    }
}

*/
