using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Utils.Scripts;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.Global.Base.Fade.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class Fade : MonoBehaviour
    {
        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;
        public string displayPropertyName;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
        }

        public async UniTask ProcessWithFade(Func<UniTask> task, CancellationToken token = new CancellationToken())
        {
            _animator.SetBool(displayPropertyName, true);
            await _stateMachine.CompleteOnAnimationFinish(0, "表示", token);
            if (token.IsCancellationRequested) return;
            await task();
            if (token.IsCancellationRequested) return;
            _animator.SetBool(displayPropertyName, false);
            await _stateMachine.CompleteOnAnimationFinish(0, "非表示", token);
        }
    }
}