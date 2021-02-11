using MageSimulator.Combo;
using MageSimulator.Combo.Components;
using MageSimulator.Global.Input.Controller.Wiimote.MotionDetector;
using MageSimulator.Scenes.Magic.Scripts.Animation;
using UniRx;
using UnityEngine.InputSystem;

namespace MageSimulator.Scenes.Magic.Scripts.Combo
{
    public class MagicSelectSubmit : Submit
    {
        public MagicAnimator.ActiveActionType actionType;
        public RestMotionDetector motion;
        public InputAction action;

        private MagicAnimator _animator;

        protected override void Start()
        {
            _animator = FindObjectOfType<MagicAnimator>();
            base.Start();
        }

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
            motion.inCondition.Subscribe(x => _animator.SetActiveAction(actionType, x)).AddTo(this);

            Observable.FromEvent<InputAction.CallbackContext>(h => action.performed += h, h => action.performed -= h)
                .Where(_ => motion.inCondition.Value)
                .Subscribe(_ => Publish())
                .AddTo(this);

            base.Initialize();
        }
    }
}