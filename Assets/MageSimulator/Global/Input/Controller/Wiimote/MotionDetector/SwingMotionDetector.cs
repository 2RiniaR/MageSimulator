using UniRx;
using UnityEngine;

namespace MageSimulator.Global.Input.Controller.Wiimote.MotionDetector
{
    public class SwingMotionDetector : MotionDetector
    {
        [Header("Wiiリモコンを振る")]
        public float swingMagnitude = 0.2f;

        [Header("Wiiリモコンを振った後の最終地点")]
        public Quaternion finishRotation;
        public bool enableFinishRotationLimit;
        public float finishPitchErrorLimit = 15f;
        public float finishYawErrorLimit = 15f;
        public float finishRollErrorLimit = 90f;
        public float finishMarginTime = 0f;

        private readonly BoolReactiveProperty _inCondition = new BoolReactiveProperty(false);
        private readonly AccelAverage _accelAverage = new AccelAverage();
        private float _swingMarginTime = 0f;

        protected override void Start()
        {
            base.Start();

            _inCondition.Pairwise()
                .Where(x => x.Previous ^ x.Current)
                .Subscribe(_ => Detect())
                .AddTo(this);
        }

        private void Update()
        {
            _accelAverage.Update(WiimoteDevice.GetAccelVector());
            if (_accelAverage.GetMagnitude() > swingMagnitude)
                _swingMarginTime = finishMarginTime;
            _inCondition.Value = _swingMarginTime > 0f && InFinishCondition();
            _swingMarginTime = Mathf.Max(0f, _swingMarginTime - Time.deltaTime);
        }

        private bool InFinishCondition()
        {
            var r1 = WiimoteDevice.Rotation * Vector3.forward;
            var r2 = finishRotation * Vector3.forward;
            var pitchAngleError = Vector3.Angle(r1, Vector3.ProjectOnPlane(r2, WiimoteDevice.Rotation * Vector3.right));
            var yawAngleError = Vector3.Angle(r1, Vector3.ProjectOnPlane(r2, WiimoteDevice.Rotation * Vector3.up));
            var rollAngleError = Vector3.Angle(WiimoteDevice.Rotation * Vector3.up, Vector3.ProjectOnPlane(r2, WiimoteDevice.Rotation * Vector3.forward));
            var isRotationAllowed = pitchAngleError < finishPitchErrorLimit && yawAngleError < finishYawErrorLimit && rollAngleError < finishRollErrorLimit;

            return !enableFinishRotationLimit || isRotationAllowed;
        }
    }
}