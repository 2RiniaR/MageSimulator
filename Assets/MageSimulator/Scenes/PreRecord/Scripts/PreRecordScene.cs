using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.DataCollection.Components.SpeakIndicator.Scripts;
using MageSimulator.DataCollection.Scripts;
using MageSimulator.Talk.Scripts;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.Scenes.PreRecord.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class PreRecordScene : MonoBehaviour
    {
        [Serializable]
        public struct StateComposite
        {
            [Header("状態")]
            public string talk0;
            public string talk1;
            public string recognize1;
            public string talk2;
            public string recognize2;
            public string talk3;
            public string recognize3;
            public string talk4;
            public string recognize4;
            public string talk5;
            public string recognize5;
            public string talk6;
            public string recognize6;
            public string talk7;

            [Header("トリガー")]
            public string nextTriggerName;
        }

        [Serializable]
        public struct AssetsComposite
        {
            [Header("会話")]
            public TalkSection talk0;
            public TalkSection talk1;
            public TalkSection talk2;
            public TalkSection talk3;
            public TalkSection talk4;
            public TalkSection talk5;
            public TalkSection talk6;
            public TalkSection talk7;
            public TalkDisplay talkDisplay;

            [Header("音声認識")]
            public JuliusGrammar recognize1;
            public JuliusGrammar recognize2;
            public JuliusGrammar recognize3;
            public JuliusGrammar recognize4;
            public JuliusGrammar recognize5;
            public JuliusGrammar recognize6;
            public SpeakIndicator speakIndicator;
        }

        public StateComposite states;
        public AssetsComposite assets;

        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;
        private CancellationTokenSource _onExitState = new CancellationTokenSource();

        public void Start()
        {
            _animator = GetComponent<Animator>();
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
            if (_stateMachine == null)
                Debug.LogWarning("Animator に ObservableStateMachineTrigger が存在しません");

            #region イベント購読

            // 何らかの状態が終了したとき
            _stateMachine.OnStateExitAsObservable()
                .Subscribe(_ =>
                {
                    _onExitState?.Cancel();
                    _onExitState?.Dispose();
                    _onExitState = new CancellationTokenSource();
                });

            // 状態「会話0」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk0))
                .Subscribe(async _ => await Talk(assets.talk0))
                .AddTo(this);

            // 状態「会話1」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk1))
                .Subscribe(async _ => await Talk(assets.talk1))
                .AddTo(this);

            // 状態「音声認識1」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.recognize1))
                .Subscribe(async _ => await Record(assets.recognize1))
                .AddTo(this);

            // 状態「会話2」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk2))
                .Subscribe(async _ => await Talk(assets.talk2))
                .AddTo(this);

            // 状態「音声認識2」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.recognize2))
                .Subscribe(async _ => await Record(assets.recognize2))
                .AddTo(this);

            // 状態「会話3」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk3))
                .Subscribe(async _ => await Talk(assets.talk3))
                .AddTo(this);

            // 状態「音声認識3」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.recognize3))
                .Subscribe(async _ => await Record(assets.recognize3))
                .AddTo(this);

            // 状態「会話4」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk4))
                .Subscribe(async _ => await Talk(assets.talk4))
                .AddTo(this);

            // 状態「音声認識4」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.recognize4))
                .Subscribe(async _ => await Record(assets.recognize4))
                .AddTo(this);

            // 状態「会話5」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk5))
                .Subscribe(async _ => await Talk(assets.talk5))
                .AddTo(this);

            // 状態「音声認識5」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.recognize5))
                .Subscribe(async _ => await Record(assets.recognize5))
                .AddTo(this);

            // 状態「会話6」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk6))
                .Subscribe(async _ => await Talk(assets.talk6))
                .AddTo(this);

            // 状態「音声認識6」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.recognize6))
                .Subscribe(async _ => await Record(assets.recognize6))
                .AddTo(this);

            // 状態「会話7」が開始されたとき
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.talk7))
                .Subscribe(async _ => await Talk(assets.talk7))
                .AddTo(this);

            #endregion
        }

        private void OnDestroy()
        {
            _onExitState?.Cancel();
            _onExitState?.Dispose();
        }

        private void Next()
        {
            _animator.SetTrigger(states.nextTriggerName);
        }

        private async UniTask Talk(TalkSection section)
        {
            await assets.talkDisplay.RunTalkSection(section).WithCancellation(_onExitState.Token);
            Next();
        }

        private async UniTask Record(JuliusGrammar grammar, string savePath = null)
        {
            var recording = await assets.speakIndicator.Record(grammar, _onExitState.Token);
            if (savePath != null)
            {
                WavUtility.SaveAsFile(recording, savePath);
                Debug.Log("Recording was saved at " + savePath);
            }
            Next();
        }
    }
}
