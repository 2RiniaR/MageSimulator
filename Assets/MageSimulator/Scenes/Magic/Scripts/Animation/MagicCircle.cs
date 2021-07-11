using UnityEngine;

namespace MageSimulator.Scenes.Magic.Scripts.Animation
{
    [RequireComponent(typeof(Animator))]
    public class MagicCircle : MonoBehaviour
    {
        public string fireTriggerName = "Fire";
        public string cancelTriggerName = "Cancel";
        public string validFlagName = "Valid";

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
        }

        public void Fire()
        {
            _animator.SetTrigger(fireTriggerName);
        }

        public void Cancel()
        {
            _animator.SetTrigger(cancelTriggerName);
        }

        public void SetValid(bool isValid)
        {
            _animator.SetBool(validFlagName, isValid);
        }
    }
}