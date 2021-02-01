using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Controller.Wiimote.MotionDetector
{
    public class RestMotionDetector : MotionDetector
    {
        // step1: (0, -1, 0)
        // step2: (0, 0, -1)
        // step3: (-1, 0, 0)
        // step4: (0, 1, 0)
        public Vector3 stayAccelValue = new Vector3(0f, -1f, 0f);
        public float moveThreshold = 0.03f;
        public float rotationThreshold = 0.5f;

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

        private void Start()
        {
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
            var accel = WiimoteDevice.GetAccelVector();
            var rotation = (accel - stayAccelValue).magnitude;
            return move < moveThreshold && rotation < rotationThreshold;
        }

        private void OnAccelPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device != WiimoteDevice) return;
            var accel = ctx.ReadValue<Vector3>();
            _accelAverage.Update(accel);
        }
    }
}