using UnityEngine.InputSystem;

namespace MageSimulator.Interactive
{
    public class SimplePage : Page
    {
        public InputAction submitAction;
        public InputAction cancelAction;

        private void OnEnable()
        {
            submitAction.Enable();
            cancelAction.Enable();
            submitAction.performed += OnSubmit;
            cancelAction.performed += OnCancel;
        }

        private void OnDisable()
        {
            submitAction.performed -= OnSubmit;
            cancelAction.performed -= OnCancel;
            submitAction.Disable();
            cancelAction.Disable();
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            MoveToNext();
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            MoveToPrevious();
        }
    }
}