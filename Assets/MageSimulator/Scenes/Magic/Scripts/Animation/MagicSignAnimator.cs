using UnityEngine;
using UnityEngine.Serialization;

namespace MageSimulator.Scenes.Magic.Scripts.Animation
{
    [RequireComponent(typeof(Animator))]
    public class MagicSignAnimator : MonoBehaviour
    {
        public string activeParameterName;
        public string chantTriggerName;
        public string fireTriggerName;
        [SerializeField] private Animator animator;

        public void SetActive(bool isActive)
        {
            animator.SetBool(activeParameterName, isActive);
        }

        public void Chanted()
        {
            animator.SetTrigger(chantTriggerName);
        }

        public void Fire()
        {
            animator.SetTrigger(fireTriggerName);
        }
    }
}