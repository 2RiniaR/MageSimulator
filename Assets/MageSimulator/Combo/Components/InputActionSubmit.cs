using UniRx;
using UnityEngine.InputSystem;

namespace MageSimulator.Combo.Components
{
    public class InputActionSubmit : Submit
    {
        public InputAction action;

        protected override void Initialize()
        {
            Observable.FromEvent<InputAction.CallbackContext>(h => action.performed += h, h => action.performed -= h)
                .Subscribe(_ => Publish())
                .AddTo(this);

            base.Initialize();
        }

        private void OnEnable()
        {
            action.Enable();
        }

        private void OnDisable()
        {
            action.Disable();
        }
    }
}