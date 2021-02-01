using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.BrowserUI.Events;
using MageSimulator.Utils.Scripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.BrowserUI
{
    [RequireComponent(typeof(Animator))]
    public class PageAnimator : BrowserComponent
    {
        [Serializable]
        public class LinkAnimationBehaviour
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
        public List<LinkAnimationBehaviour> initialAnimations;
        public AnimatorBehaviour defaultFinalAnimation;
        public List<LinkAnimationBehaviour> finalAnimations;

        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected override void Initialize()
        {
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
            if (cache == null) return;

            var animatorBehaviour =
                initialAnimations.Find(x => x.matchLinkName == cache.linkName)?.animatorBehaviour ??
                defaultInitialAnimation;

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

        protected override void PassEvent(BrowserEvent e)
        {
            switch (e)
            {
                case ChangePageEvent changePage:
                    OnChangePage(changePage, _cancellationTokenSource.Token).Forget();
                    return;
                case FinishPageEvent finish:
                    OnFinish(finish, _cancellationTokenSource.Token).Forget();
                    return;
            }

            PublishEvent(e);
        }

        private async UniTask OnChangePage(ChangePageEvent e, CancellationToken token = new CancellationToken())
        {
            var cache = GetComponentInParent<PageCache>();
            if (cache != null)
                cache.linkName = e.name;

            var animatorBehaviour =
                finalAnimations.Find(x => x.matchLinkName == e.name)
                    ?.animatorBehaviour ?? defaultFinalAnimation;

            if (animatorBehaviour.waitFinish)
            {
                await PlayAnimation(animatorBehaviour, token);
                PublishEvent(new OpenPageEvent {name = e.name, target = e.target});
                PublishEvent(new ClosePageEvent {name = e.name});
            }
            else
            {
                PublishEvent(new OpenPageEvent {name = e.name, target = e.target});
                await PlayAnimation(animatorBehaviour, token);
                PublishEvent(new ClosePageEvent {name = e.name});
            }
        }

        private async UniTask OnFinish(FinishPageEvent e, CancellationToken token = new CancellationToken())
        {
            var animatorBehaviour =
                finalAnimations.Find(x => x.matchLinkName == e.name)
                    ?.animatorBehaviour ?? defaultFinalAnimation;

            await PlayAnimation(animatorBehaviour, token);
            PublishEvent(e);
        }
    }
}