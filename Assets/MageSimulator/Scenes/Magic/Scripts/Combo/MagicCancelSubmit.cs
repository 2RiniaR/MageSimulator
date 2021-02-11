using MageSimulator.Combo.Components;
using MageSimulator.Global.Input.Controller.Wiimote.MotionDetector;
using UniRx;
using UnityEngine.InputSystem;

namespace MageSimulator.Scenes.Magic.Scripts.Combo
{
    public class MagicCancelSubmit : Submit
    {
        public RestMotionDetector motion;
        public InputAction action;

        private void OnEnable()
        {
            action.Enable();
        }

        private void OnDisable()
        {
            action.Disable();
        }

        protected override void Initialize()
        {
            Observable.FromEvent<InputAction.CallbackContext>(h => action.canceled += h,
                    h => action.canceled -= h).AsUnitObservable()
                .Merge(motion.inCondition.Where(x => !x).AsUnitObservable())
                .Subscribe(_ => Publish())
                .AddTo(this);

            base.Initialize();
        }
    }
}