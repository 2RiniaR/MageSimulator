using UnityEngine;
using UnityEngine.Animations;

namespace MageSimulator.Utils.Scripts
{
    public class DestroyOnExitStateMachineBehaviour : StateMachineBehaviour
    {
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
        {
            Destroy(animator.gameObject);
        }
    }
}