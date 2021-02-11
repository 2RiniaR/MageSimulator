using System;
using System.Text;
using MageSimulator.Combo.Components;
using MageSimulator.Combo.Effects;
using MageSimulator.Combo.Events;
using UnityEngine;
using WiimoteApi;

namespace MageSimulator.Global.Input.Controller.Wiimote.Calibrator.Scripts
{
    public class WiimoteCalibrator : ComboEffect
    {
        public enum CalibrateStep
        {
            AccelAButtonUp,
            AccelExpansionUp,
            AccelLeftSideUp,
            ResetMotionPlus
        }

        public CalibrateStep step;

        protected override void PassEvent(ComboEvent e)
        {
            if (e is SubmitEvent submit)
                ApplyCalibration(submit.name);
        }

        private void ApplyCalibration(string deviceName)
        {
            var device = WiimoteExtension.GetDevice(deviceName);
            if (device == null) return;
            switch (step)
            {
                case CalibrateStep.AccelAButtonUp:
                    device.SetCalibrateStep(AccelCalibrationStep.A_BUTTON_UP);
                    break;
                case CalibrateStep.AccelExpansionUp:
                    device.SetCalibrateStep(AccelCalibrationStep.EXPANSION_UP);
                    break;
                case CalibrateStep.AccelLeftSideUp:
                    device.SetCalibrateStep(AccelCalibrationStep.LEFT_SIDE_UP);
                    break;
                case CalibrateStep.ResetMotionPlus:
                    device.ResetRotation(Quaternion.identity * Quaternion.FromToRotation(Vector3.up, Vector3.left) *
                                         Quaternion.FromToRotation(Vector3.up, Vector3.left));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var str = new StringBuilder();
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    str.Append(device.Wiimote.Accel.accel_calib[y, x]).Append(" ");
                }
                str.Append("\n");
            }
            Debug.Log(str.ToString());
        }
    }
}