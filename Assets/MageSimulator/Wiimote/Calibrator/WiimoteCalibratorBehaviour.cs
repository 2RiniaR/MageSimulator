using MageSimulator.Wiimote.Scripts;
using UnityEngine;
using WiimoteApi;

namespace MageSimulator.Wiimote.Calibrator
{
    public class WiimoteCalibratorBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var wiimote = FindObjectOfType<WiimoteActivator>()?.Wiimote;
            if (wiimote == null) return;
            if (stateInfo.IsName("Step1"))
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
            else if (stateInfo.IsName("Step2"))
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.EXPANSION_UP);
            else if (stateInfo.IsName("Step3"))
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.LEFT_SIDE_UP);
        }
    }
}
