using MageSimulator.Controller.Wiimote.Calibrator.Scripts.MotionDetector;
using UnityEngine;

namespace MageSimulator.Controller.Wiimote.Calibrator.Scripts
{
    public class WiimoteCalibratorStateMachineBehaviour : StateMachineBehaviour
    {
        public string activateStateName;
        public string deactivateStateName;
        public MotionType activateMotionType;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            var calibrator = animator.GetComponent<WiimoteCalibrator>();
            if (calibrator == null)
                return;
            if (stateInfo.IsName(activateStateName))
                calibrator.SetDetector(activateMotionType);
            else if (stateInfo.IsName(deactivateStateName))
                calibrator.DeactivateDetector();
        }
    }
}