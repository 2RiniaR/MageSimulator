using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MageSimulator.Interactive.Scripts
{
    public class ItemSelectPage : Page
    {
        public InputAction submitAction;
        public InputAction cancelAction;
        public InputAction selectAction;
        public int itemCount = 2;
        public int selectingItem = 0;

        public Animator animator;
        public string selectingItemAnimatorParameterName;
        public UnityEvent<int> onSelectItemChanged;

        private void OnEnable()
        {
            submitAction.Enable();
            cancelAction.Enable();
            selectAction.Enable();
            submitAction.performed += OnSubmit;
            cancelAction.performed += OnCancel;
            selectAction.performed += OnSelect;
            animator.SetInteger(selectingItemAnimatorParameterName, selectingItem);
            onSelectItemChanged.Invoke(selectingItem);
        }

        private void OnDisable()
        {
            submitAction.performed -= OnSubmit;
            cancelAction.performed -= OnCancel;
            selectAction.performed -= OnSelect;
            submitAction.Disable();
            cancelAction.Disable();
            selectAction.Disable();
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            MoveToNext();
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            MoveToPrevious();
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            if (itemCount == 0) return;

            var value = ctx.ReadValue<float>();
            if (value < 0)
                selectingItem = (selectingItem + (itemCount - 1)) % itemCount;
            else
                selectingItem = (selectingItem + 1) % itemCount;

            animator.SetInteger(selectingItemAnimatorParameterName, selectingItem);
            onSelectItemChanged.Invoke(selectingItem);
        }
    }
}
