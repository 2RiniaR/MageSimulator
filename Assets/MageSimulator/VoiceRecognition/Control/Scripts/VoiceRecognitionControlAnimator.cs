using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.VoiceRecognition.Envelope;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.VoiceRecognition.Control.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class VoiceRecognitionControlAnimator : MonoBehaviour
    {
        [Serializable]
        public struct AnimatorTriggerComposite
        {
            public string inactive;
            public string setting;
            public string active;
            public string ready;
            public string recognizing;
            public string failed;
            public string succeed;
        }

        /// <summary>
        ///     アニメーションとの連携
        /// </summary>
        public AnimatorTriggerComposite animatorTriggers = new AnimatorTriggerComposite
        {
            inactive = "Inactive",
            setting = "Setting",
            active = "Active",
            ready = "Ready",
            recognizing = "Recognizing",
            failed = "Failed",
            succeed = "Succeed"
        };

        /// <summary>
        ///     認識する文章を表示するテキストコンポーネント
        /// </summary>
        public TMP_Text grammarText;

        /// <summary>
        ///     音声認識を実行するコンポーネント
        /// </summary>
        public VoiceRecognitionControl control;

        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;

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
            OnControlStateEnterObservable(VoiceRecognitionControl.State.Inactive)
                .Subscribe(_ => _animator.SetTrigger(animatorTriggers.inactive))
                .AddTo(this);

            OnControlStateEnterObservable(VoiceRecognitionControl.State.Setting)
                .Subscribe(_ => _animator.SetTrigger(animatorTriggers.setting))
                .AddTo(this);

            OnControlStateEnterObservable(VoiceRecognitionControl.State.Active)
                .Subscribe(_ => _animator.SetTrigger(animatorTriggers.active))
                .AddTo(this);

            OnControlStateEnterObservable(VoiceRecognitionControl.State.Ready)
                .Subscribe(_ => _animator.SetTrigger(animatorTriggers.ready))
                .AddTo(this);

            OnControlStateEnterObservable(VoiceRecognitionControl.State.Recognizing)
                .Subscribe(_ => _animator.SetTrigger(animatorTriggers.recognizing))
                .AddTo(this);

            control.OnRecognized
                .Where(x => x)
                .Subscribe(_ => _animator.SetTrigger(animatorTriggers.succeed))
                .AddTo(this);

            control.OnRecognized
                .Where(x => !x)
                .Subscribe(_ => _animator.SetTrigger(animatorTriggers.failed))
                .AddTo(this);

            control.OnGrammarChangedObservable
                .Where(grammar => grammar != null)
                .Subscribe(grammar => grammarText.text = grammar.display)
                .AddTo(this);
        }

        private IObservable<Unit> OnControlStateEnterObservable(VoiceRecognitionControl.State state)
        {
            return control.OnStateChangedObservable.Where(x => x == state).AsUnitObservable();
        }
    }
}
