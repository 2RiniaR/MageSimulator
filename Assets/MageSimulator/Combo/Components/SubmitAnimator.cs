using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Events;
using MageSimulator.Combo.Utils;
using MageSimulator.Utils.Scripts;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.Combo.Components
{
    [RequireComponent(typeof(Animator))]
    public class SubmitAnimator : ComboComponent
    {
        [Serializable]
        public class SubmitAnimationBehaviour
        {
            public string matchLinkName;
            public AnimatorBehaviour animatorBehaviour;
        }

        [Serializable]
        public class AnimatorBehaviour
        {
            public string triggerName;
            public string destinationLayerName;
            public string destinationStateName;
            public bool waitFinish;
        }

        public AnimatorBehaviour defaultInitialAnimation;
        public List<SubmitAnimationBehaviour> initialAnimations;
        public AnimatorBehaviour defaultFinalAnimation;
        public List<SubmitAnimationBehaviour> finalAnimations;

        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected override void Initialize()
        {
            InitializeEffects();
            OnInitialize(_cancellationTokenSource.Token).Forget();
        }

        protected override void Start()
        {
            _cancellationTokenSource.CancelWith(this);

            _animator = GetComponent<Animator>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (_stateMachine == null)
                Debug.LogWarning("Animator に ObservableStateMachineTrigger が存在しません");

            base.Start();
        }

        private async UniTask OnInitialize(CancellationToken token = new CancellationToken())
        {
            var cache = GetComponentInParent<PageCache>();

            AnimatorBehaviour animatorBehaviour;
            if (cache == null)
            {
                animatorBehaviour = defaultInitialAnimation;
            }
            else
            {
                animatorBehaviour =
                    initialAnimations.Find(x => x.matchLinkName == cache.linkName)?.animatorBehaviour ??
                    defaultInitialAnimation;
            }

            if (animatorBehaviour.waitFinish)
            {
                InitializeChildren();
                await PlayAnimation(animatorBehaviour, token);
            }
            else
            {
                await PlayAnimation(animatorBehaviour, token);
                InitializeChildren();
            }
        }

        private UniTask PlayAnimation(AnimatorBehaviour animatorBehaviour, CancellationToken token = new CancellationToken())
        {
            if (string.IsNullOrEmpty(animatorBehaviour.triggerName)) return UniTask.CompletedTask;

            _animator.SetTrigger(animatorBehaviour.triggerName);
            return _stateMachine.CompleteOnAnimationFinish(
                _animator.GetLayerIndex(animatorBehaviour.destinationLayerName),
                animatorBehaviour.destinationStateName, token);
        }

        protected override void PassEvent(ComboEvent e)
        {
            PassEventEffects(e);
            if (e is SubmitEvent submit)
                OnSubmit(submit, _cancellationTokenSource.Token).Forget();
            else
                PublishEvent(e);
        }

        private async UniTask OnSubmit(SubmitEvent e, CancellationToken token = new CancellationToken())
        {
            var animatorBehaviour =
                finalAnimations.Find(x => x.matchLinkName == e.name)
                    ?.animatorBehaviour ?? defaultFinalAnimation;

            if (animatorBehaviour.waitFinish)
            {
                await PlayAnimation(animatorBehaviour, token);
                PublishEvent(e);
            }
            else
            {
                PublishEvent(e);
                await PlayAnimation(animatorBehaviour, token);
            }
        }
    }
}