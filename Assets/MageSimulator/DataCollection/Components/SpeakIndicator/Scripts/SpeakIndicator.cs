using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.DataCollection.Scripts;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.DataCollection.Components.SpeakIndicator.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class SpeakIndicator : MonoBehaviour
    {
        [Serializable]
        public struct StateComposite
        {
            [Header("状態")]
            public string inactivateStateName;
            public string waitingAudioInputStateName;
            public string recordingStateName;

            [Header("トリガー")]
            public string activationStartedTriggerName;
            public string readyRecordingTriggerName;
            public string actionStartedTriggerName;
            public string actionCanceledTriggerName;
            public string recordingStartedTriggerName;
            public string recognitionSucceedTriggerName;
            public string recognitionFailedTriggerName;
            public string resetTriggerName;
        }

        public StateComposite states;
        public InputAction readyAction;
        public TextMeshProUGUI grammarText;
        public DataCollector dataCollector;

        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;

        private void OnEnable()
        {
            readyAction.Enable();
        }

        private void OnDisable()
        {
            readyAction.Disable();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
            if (_stateMachine == null)
                Debug.LogWarning("Animator に ObservableStateMachineTrigger が存在しません");
        }

        private void Start()
        {
            dataCollector.OnInputStarted
                .ObserveOnMainThread()
                .Where(_ => _animator.GetCurrentAnimatorStateInfo(0).IsName(states.waitingAudioInputStateName))
                .Subscribe(_ => OnRecordStarted())
                .AddTo(this);

            dataCollector.OnRetry
                .ObserveOnMainThread()
                .Where(_ => _animator.GetCurrentAnimatorStateInfo(0).IsName(states.recordingStateName))
                .Subscribe(_ => OnRecognitionFailed())
                .AddTo(this);
        }

        /// <summary>
        ///     録音を実行する
        /// </summary>
        public async UniTask<AudioClip> Record(JuliusGrammar grammar, CancellationToken token = new CancellationToken())
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(states.inactivateStateName))
                return null;

            grammarText.text = grammar.display;
            _animator.SetTrigger(states.activationStartedTriggerName);

            await dataCollector.SetGrammar(grammar, token);
            await UniTask.SwitchToMainThread();
            _animator.SetTrigger(states.readyRecordingTriggerName);

            while (true)
            {
                await Observable.FromEvent<InputAction.CallbackContext>(
                    h => readyAction.started += h,
                    h => readyAction.started -= h)
                    .First().ToUniTask(cancellationToken: token);

                await UniTask.SwitchToMainThread();
                _animator.SetTrigger(states.actionStartedTriggerName);

                var cancel = new CancellationTokenSource();
                var cancelActionTask = Observable.FromEvent<InputAction.CallbackContext>(
                    h => readyAction.canceled += h,
                    h => readyAction.canceled -= h)
                .First().ToUniTask(cancellationToken: cancel.Token);
                var recordTask = dataCollector.Record(cancel.Token);

                var (win, _, result) = await UniTask.WhenAny(cancelActionTask, recordTask);
                cancel.Cancel();

                await UniTask.SwitchToMainThread();
                if (win == 1)
                {
                    _animator.SetTrigger(states.recognitionSucceedTriggerName);
                    return result;
                }

                _animator.SetTrigger(states.actionCanceledTriggerName);
            }
        }

        public void Reset()
        {
            _animator.SetTrigger(states.resetTriggerName);
        }

        private void OnRecordStarted()
        {
            if (_animator == null) return;
            _animator.SetTrigger(states.recordingStartedTriggerName);
        }

        private void OnRecognitionFailed()
        {
            if (_animator == null) return;
            _animator.SetTrigger(states.recognitionFailedTriggerName);
        }
    }
}
