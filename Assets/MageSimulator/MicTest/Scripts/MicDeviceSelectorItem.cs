using UnityEngine;

namespace MageSimulator.MicTest.Scripts
{
    public class MicDeviceSelectorItem : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI text;
        public Animator animator;
        public string activeAnimatorParameterName;
        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        public void SetActive(bool isActive)
        {
            animator.SetBool(activeAnimatorParameterName, isActive);
        }
    }
}
