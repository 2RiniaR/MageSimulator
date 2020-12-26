using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MageSimulator.MicTest.Scripts
{
    public class MicGainSetter : MonoBehaviour
    {
        [Min(0f)] public float minRate;
        [Min(0f)] public float maxRate;
        [Min(0f)] public float stepRate;
        [Min(0f)] public float rate;
        public UnityEvent<float> onRateChanged;

        public InputAction selectAction;
        public TextMeshProUGUI rateText;

        [Header("アニメーション設定")]
        public Animator animator;
        public string decreaseRateAnimatorParameterName;
        public string increaseRateAnimatorParameterName;

        private void OnEnable()
        {
            selectAction.Enable();
            selectAction.performed += OnSelect;
        }

        private void OnDisable()
        {
            selectAction.performed -= OnSelect;
            selectAction.Disable();
        }

        private void Start()
        {
            SetRateText(rate);
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            if (value > 0)
            {
                rate = (Mathf.FloorToInt(rate / stepRate) + 1) * stepRate;
                animator.SetTrigger(increaseRateAnimatorParameterName);
            }
            else
            {
                rate = (Mathf.FloorToInt(rate / stepRate) - 1) * stepRate;
                animator.SetTrigger(decreaseRateAnimatorParameterName);
            }
            rate = Mathf.Clamp(rate, minRate, maxRate);
            onRateChanged.Invoke(rate);
            SetRateText(rate);
        }

        private void SetRateText(float value)
        {
            rateText.text = "x " + value.ToString("F1");
        }
    }
}
