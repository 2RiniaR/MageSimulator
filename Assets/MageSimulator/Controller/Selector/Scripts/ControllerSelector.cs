using MageSimulator.BrowserUI;
using MageSimulator.Config.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MageSimulator.Controller.Selector.Scripts
{
    public class ControllerSelector : BrowserComponent
    {
        public InputAction selectKeyboardAction;
        public InputAction selectWiimoteAction;
        public Animator animator;
        public string selectKeyboardAnimatorTriggerName;
        public string selectWiimoteAnimatorTriggerName;
        public UnityEvent onKeyboardSelected;
        public UnityEvent onWiimoteSelected;
        private ApplicationSettings _applicationSettings;

        private void Awake()
        {
            _applicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
        }

        private void OnEnable()
        {
            selectKeyboardAction.Enable();
            selectWiimoteAction.Enable();
            selectKeyboardAction.performed += OnSelectKeyboard;
            selectWiimoteAction.performed += OnSelectWiimote;
        }

        private void OnDisable()
        {
            selectKeyboardAction.performed -= OnSelectKeyboard;
            selectWiimoteAction.performed -= OnSelectWiimote;
            selectKeyboardAction.Disable();
            selectWiimoteAction.Disable();
        }

        private void OnSelectKeyboard(InputAction.CallbackContext ctx)
        {
            _applicationSettings.deviceType = ControllerDeviceType.KeyBoard;
            animator.SetTrigger(selectKeyboardAnimatorTriggerName);
            onKeyboardSelected.Invoke();
        }

        private void OnSelectWiimote(InputAction.CallbackContext ctx)
        {
            _applicationSettings.deviceType = ControllerDeviceType.Wiimote;
            _applicationSettings.wiimotePid = ctx.control.device.description.product;
            animator.SetTrigger(selectWiimoteAnimatorTriggerName);
            onWiimoteSelected.Invoke();
        }
    }
}