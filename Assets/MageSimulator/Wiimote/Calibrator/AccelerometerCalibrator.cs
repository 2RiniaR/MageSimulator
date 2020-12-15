using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using WiimoteApi;

namespace MageSimulator.Wiimote.Calibrator
{
    public class AccelerometerCalibrator : MonoBehaviour
    {
        public Animator animator;
        public string openAnimatorTriggerName;
        public string submitAnimatorTriggerName;
        public string cancelAnimatorTriggerName;
        public InputAction openAction;
        public InputAction submitAction;
        public InputAction cancelAction;

        private void Start()
        {
            openAction.Enable();
            submitAction.Enable();
            cancelAction.Enable();
            openAction.performed += OnOpen;
            submitAction.performed += OnSubmit;
            cancelAction.performed += OnCancel;
        }

        private void OnOpen(InputAction.CallbackContext ctx)
        {
            animator.SetTrigger(openAnimatorTriggerName);
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            animator.SetTrigger(submitAnimatorTriggerName);
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            animator.SetTrigger(cancelAnimatorTriggerName);
        }

    }
}