using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Utils.Scripts
{
    public class ItemSelector : MonoBehaviour
    {
        public IObservable<int> OnValueChanged => _onValueChanged;
        public IObservable<int> OnSubmitted => _onSubmitted;
        public int Current { get; private set; } = 0;
        public InputAction plus;
        public InputAction minus;
        public int min = 0;
        public int max = 1;
        public int initialValue = 0;
        public bool cycle = true;
        public string selectAnimatorParameterName = "select";
        public string submitAnimatorTriggerName = "submit";
        private Animator _animator;
        private readonly Subject<int> _onValueChanged = new Subject<int>();
        private readonly Subject<int> _onSubmitted = new Subject<int>();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            plus.Enable();
            minus.Enable();
        }

        private void OnDisable()
        {
            plus.Disable();
            minus.Disable();
        }

        private void Start()
        {
            SetValue(initialValue);

            Observable.FromEvent<InputAction.CallbackContext>(
                h => plus.performed += h,
                h => plus.performed -= h)
                .Subscribe(_ => Increment())
                .AddTo(this);

            Observable.FromEvent<InputAction.CallbackContext>(
                h => minus.performed += h,
                h => minus.performed -= h)
                .Subscribe(_ => Decrement())
                .AddTo(this);
        }

        public void SetValue(int value)
        {
            Current = Mathf.Clamp(value, min, max);
            _onValueChanged.OnNext(Current);
            if (_animator != null && !string.IsNullOrEmpty(selectAnimatorParameterName))
                _animator.SetInteger(selectAnimatorParameterName, Current);
        }

        public void Submit()
        {
            _onSubmitted.OnNext(Current);
            if (_animator != null && !string.IsNullOrEmpty(submitAnimatorTriggerName))
                _animator.SetTrigger(submitAnimatorTriggerName);
        }

        public void Increment()
        {
            Add(1);
        }

        public void Decrement()
        {
            Add(-1);
        }

        public void Add(int count)
        {
            if (count > 0)
            {
                if (cycle)
                    SetValue((Current - min + count) % (max - min) + min);
                else
                    SetValue(Current + count);
            }
            else
            {
                if (cycle)
                    SetValue((Current - min + (max - min + count - 1)) % (max - min) + min);
                else
                    SetValue(Current + count);
            }
        }
    }
}