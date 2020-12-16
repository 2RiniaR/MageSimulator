using System;
using System.Collections;
using System.Linq;
using MageSimulator.Wiimote.Scripts;
using TMPro;
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

        public TextMeshProUGUI accelText;
        public TextMeshProUGUI motionText;

        private static readonly int Step1StateHash = Animator.StringToHash("Base Layer.Step1");
        private static readonly int Step2StateHash = Animator.StringToHash("Base Layer.Step2");
        private static readonly int Step3StateHash = Animator.StringToHash("Base Layer.Step3");
        private static readonly int HiddenStateHash = Animator.StringToHash("Base Layer.Hidden");

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
            var currentStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (currentStateHash != HiddenStateHash) return;
            animator.SetTrigger(openAnimatorTriggerName);
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            var currentStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            var wiimote = WiimoteActivator.Instance.Wiimote;
            if (wiimote == null) return;
            if (currentStateHash == Step1StateHash)
            {
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
                WiimoteActivator.Instance.ResetRotation();
                Debug.Log("Wiimote accelerometer calibrated: A_BUTTON_UP");
                animator.SetTrigger(submitAnimatorTriggerName);
            } else if (currentStateHash == Step2StateHash)
            {
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.EXPANSION_UP);
                Debug.Log("Wiimote accelerometer calibrated: EXPANSION_UP");
                animator.SetTrigger(submitAnimatorTriggerName);
            }
            else if (currentStateHash == Step3StateHash)
            {
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.LEFT_SIDE_UP);
                Debug.Log("Wiimote accelerometer calibrated: LEFT_SIDE_UP");
                animator.SetTrigger(submitAnimatorTriggerName);
            }
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            var currentStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (new[] {Step1StateHash, Step2StateHash, Step3StateHash}.Contains(currentStateHash))
            {
                animator.SetTrigger(cancelAnimatorTriggerName);
            }
        }

        public void Update()
        {
            if (WiimoteActivator.Instance.Wiimote == null) return;
            var accel = WiimoteActivator.Instance.GetAccelVector();
            var roll = WiimoteActivator.Instance.GetRollVector();
            accelText.text = "X: " + accel.x.ToString("F3") + ", Y: " + accel.y.ToString("F3") + ", Z: " +
                             accel.z.ToString("F3");
            motionText.text = "P: " + roll.x.ToString("F3") + ", Y: " + roll.y.ToString("F3") + ", R: " +
                              roll.z.ToString("F3");
        }
    }
}