using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Utils.Scripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.BrowserUI
{
    public class LinkAnimator : BrowserComponent
    {
        [Serializable]
        public struct AnimationBehaviour
        {
            public string matchLinkName;
            public string animatorTriggerName;
            public string animatorDestinationLayerName;
            public string animatorDestinationStateName;
        }

        public List<AnimationBehaviour> initialAnimations;
        public List<AnimationBehaviour> finalAnimations;

        private bool _linkFlag = false;
        private readonly AsyncSubject<Unit> _awoke = new AsyncSubject<Unit>();
        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelWith(this);

            _animator = GetComponent<Animator>();
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
            if (_stateMachine == null)
                Debug.LogWarning("Animator に ObservableStateMachineTrigger が存在しません");

            _awoke.OnNext(Unit.Default);
            _awoke.OnCompleted();
        }

        protected override async UniTask OnInitializeComponent(CancellationToken token = new CancellationToken())
        {
            await _awoke;

            var link = initialAnimations.Find(x => x.matchLinkName == Browser.GetCache(Link.CacheKey));
            if (string.IsNullOrEmpty(link.matchLinkName)) return;

            _animator.SetTrigger(link.animatorTriggerName);
            await _stateMachine.CompleteOnAnimationFinish(
                _animator.GetLayerIndex(link.animatorDestinationLayerName), link.animatorDestinationStateName,
                _cancellationTokenSource.Token);
        }

        protected override async UniTask OnClosePage(Link source, CancellationToken token = new CancellationToken())
        {
            if (_linkFlag) return;
            _linkFlag = true;

            var link = finalAnimations.Find(x => x.matchLinkName == source.linkName);
            if (string.IsNullOrEmpty(link.animatorTriggerName)) return;

            _animator.SetTrigger(link.animatorTriggerName);
            await _stateMachine.CompleteOnAnimationFinish(
                _animator.GetLayerIndex(link.animatorDestinationLayerName), link.animatorDestinationStateName,
                _cancellationTokenSource.Token);
        }
    }
}