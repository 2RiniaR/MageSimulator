using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Controller.Wiimote.Calibrator.Scripts.MotionDetector
{
    public class Step2MotionDetector : MotionDetector
    {
        private static readonly Vector3 StayAccelValue = new Vector3(0f, 0f, -1f);
        private const float MoveThreshold = 0.03f;
        private const float RotationThreshold = 0.5f;
        private InputAction _accelAction;
        private readonly AccelAverage _accelAverage = new AccelAverage();

        private void Awake()
        {
            _accelAction = new InputAction("accel", binding: "<WiimoteDevice>/Accelerometer");
        }

        private void OnEnable()
        {
            _accelAction.Enable();
            _accelAction.performed += OnAccelPerformed;
        }

        private void OnDisable()
        {
            _accelAction.performed -= OnAccelPerformed;
            _accelAction?.Disable();
        }

        public override bool InCondition()
        {
            var move = _accelAverage.GetMagnitude();
            var accel = WiimoteDevice.GetAccelVector();
            var rotation = (accel - StayAccelValue).magnitude;
            return move < MoveThreshold && rotation < RotationThreshold;
        }

        private void OnAccelPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device != WiimoteDevice)
                return;
            var accel = ctx.ReadValue<Vector3>();
            _accelAverage.Update(accel);
        }
    }
}