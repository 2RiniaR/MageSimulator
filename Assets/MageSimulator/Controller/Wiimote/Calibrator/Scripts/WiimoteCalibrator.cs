using System;
using MageSimulator.BrowserUI;
using MageSimulator.BrowserUI.Events;
using WiimoteApi;

namespace MageSimulator.Controller.Wiimote.Calibrator.Scripts
{
    public class WiimoteCalibrator : BrowserComponent
    {
        public enum CalibrateStep
        {
            AccelAButtonUp,
            AccelExpansionUp,
            AccelLeftSideUp,
            ResetMotionPlus
        }

        public CalibrateStep step;

        protected override void PassEvent(BrowserEvent e)
        {
            if (e is SubmitEvent submit)
                ApplyCalibration(submit.name);

            PublishEvent(e);
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
                    device.ResetRotation();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}