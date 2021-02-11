using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Global.Input.Controller.Wiimote.MotionDetector
{
    public class RestMotionDetector : MotionDetector
    {
        public Vector3 stayAccel = new Vector3(0f, -1f, 0f);
        public Quaternion stayRotation;
        public bool enableRotationLimit;
        public float pitchErrorLimit = 15f;
        public float yawErrorLimit = 15f;
        public float rollErrorLimit = 90f;
        public bool enableMoveLimit;
        public float moveLimit = 0.03f;
        public bool enableAccelLimit;
        public float accelErrorLimit = 0.5f;

        public float requireTime = 2f;
        public FloatReactiveProperty continueTime;
        public BoolReactiveProperty inCondition;

        private InputAction _accelAction;
        private readonly AccelAverage _accelAverage = new AccelAverage();

        private void Awake()
        {
            _accelAction = new InputAction("accel", binding: "<WiimoteDevice>/Accelerometer");
        }

        private void OnEnable()
        {
            _accelAction.Enable();
        }

        private void OnDisable()
        {
            _accelAction?.Disable();
        }

        protected override void Start()
        {
            base.Start();

            Observable.FromEvent<InputAction.CallbackContext>(
                    h => _accelAction.performed += h,
                    h => _accelAction.performed -= h)
                .Subscribe(OnAccelPerformed).AddTo(this);

            continueTime.Pairwise()
                .Where(x => x.Previous < requireTime && requireTime <= x.Current)
                .Subscribe(_ => Detect())
                .AddTo(this);

            continueTime.Value = 0f;
        }

        private void Update()
        {
            inCondition.Value = WiimoteDevice != null && InCondition();
            if (inCondition.Value)
                continueTime.Value += Time.deltaTime;
            else
                continueTime.Value = 0;
        }

        private bool InCondition()
        {
            var move = _accelAverage.GetMagnitude();
            var accel = (WiimoteDevice.GetAccelVector() - stayAccel).magnitude;

            var r1 = WiimoteDevice.Rotation * Vector3.forward;
            var r2 = stayRotation * Vector3.forward;
            var pitchAngleError = Vector3.Angle(r1, Vector3.ProjectOnPlane(r2, WiimoteDevice.Rotation * Vector3.right));
            var yawAngleError = Vector3.Angle(r1, Vector3.ProjectOnPlane(r2, WiimoteDevice.Rotation * Vector3.up));
            var rollAngleError = Vector3.Angle(WiimoteDevice.Rotation * Vector3.up, Vector3.ProjectOnPlane(r2, WiimoteDevice.Rotation * Vector3.forward));
            var isRotationAllowed = pitchAngleError < pitchErrorLimit && yawAngleError < yawErrorLimit && rollAngleError < rollErrorLimit;

            // Debug.Log($"m: {move}  a: {accel}  p: {pitchAngleError}, y: {yawAngleError}, r: {rollAngleError}");
            return (!enableMoveLimit || move < moveLimit) && (!enableAccelLimit || accel < accelErrorLimit) &&
                   (!enableRotationLimit || isRotationAllowed);
        }

        private void OnAccelPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device != WiimoteDevice) return;
            var accel = ctx.ReadValue<Vector3>();
            _accelAverage.Update(accel);
        }
    }
}