using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Utils.Scripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.BrowserUI
{
    public class ItemSelector : BrowserComponent
    {
        [Serializable]
        public struct AnimationBehaviour
        {
            public string animatorTriggerName;
            public string animatorDestinationLayerName;
            public string animatorDestinationStateName;
        }

        public IObservable<int> OnValueChanged => _onValueChanged;
        public IObservable<int> OnSubmitted => _onSubmitted;
        public int CurrentValue { get; private set; } = 0;
        public InputAction selectAction;
        public int min = 0;
        public int max = 1;
        public int initialValue = 0;
        public bool cycle = true;
        public bool submitOnClose = false;
        public string initializeAnimatorTriggerName = "initialize";
        public string selectAnimatorParameterName = "select";
        public AnimationBehaviour submitAnimation;

        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;
        private readonly Subject<int> _onValueChanged = new Subject<int>();
        private readonly Subject<int> _onSubmitted = new Subject<int>();

        private void OnEnable()
        {
            selectAction.Enable();
        }

        private void OnDisable()
        {
            selectAction.Disable();
        }

        protected override async UniTask OnInitializeComponent(CancellationToken token = new CancellationToken())
        {
            Observable.FromEvent<InputAction.CallbackContext>(
                    h => selectAction.performed += h,
                    h => selectAction.performed -= h)
                .Subscribe(ctx =>
                {
                    var value = ctx.ReadValue<float>();
                    if (value > 0) Increment();
                    else if (value < 0) Decrement();
                })
                .AddTo(this);
        }

        protected override async UniTask OnClosePage(Link source, CancellationToken token = new CancellationToken())
        {
            if (submitOnClose)
                await Submit(token);
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
            if (_stateMachine == null)
                Debug.LogWarning("Animator に ObservableStateMachineTrigger が存在しません");

            SetValue(initialValue);
            _animator.SetTrigger(initializeAnimatorTriggerName);
        }

        public void SetValue(int value)
        {
            CurrentValue = Mathf.Clamp(value, min, max);
            _onValueChanged.OnNext(CurrentValue);
            if (_animator != null && !string.IsNullOrEmpty(selectAnimatorParameterName))
                _animator.SetInteger(selectAnimatorParameterName, CurrentValue);
        }

        public async UniTask Submit(CancellationToken token = new CancellationToken())
        {
            _onSubmitted.OnNext(CurrentValue);
            if (_animator == null || string.IsNullOrEmpty(submitAnimation.animatorTriggerName))
                return;

            _animator.SetTrigger(submitAnimation.animatorTriggerName);
            await _stateMachine.CompleteOnAnimationFinish(
                _animator.GetLayerIndex(submitAnimation.animatorDestinationLayerName),
                submitAnimation.animatorDestinationStateName, token);
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
                    SetValue((CurrentValue - min + count) % (max - min) + min);
                else
                    SetValue(CurrentValue + count);
            }
            else
            {
                if (cycle)
                    SetValue((CurrentValue - min + (max - min + count - 1)) % (max - min) + min);
                else
                    SetValue(CurrentValue + count);
            }
        }
    }
}