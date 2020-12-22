using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Scenes.PriorExplanation.Scripts
{
    public class PriorExplanation : MonoBehaviour
    {
        public Animator animator;
        public string moveToPreviousPageAnimatorTriggerName;
        public string moveToNextPageAnimatorTriggerName;
        public string exitSceneAnimatorTriggerName;

        public void MoveToPreviousPage()
        {
            animator.SetTrigger(moveToPreviousPageAnimatorTriggerName);
        }

        public void MoveToNextPage()
        {
            animator.SetTrigger(moveToNextPageAnimatorTriggerName);
        }
    }
}
