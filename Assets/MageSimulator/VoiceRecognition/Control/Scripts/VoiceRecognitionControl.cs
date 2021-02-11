using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.VoiceRecognition.Envelope;
using UniRx;
using UnityEngine;

namespace MageSimulator.VoiceRecognition.Control.Scripts
{
    [CreateAssetMenu(fileName = "New Voice Recognition Control", menuName = "Voice/VoiceRecognitionControl", order = 0)]
    public class VoiceRecognitionControl : ScriptableObject
    {
        public enum State
        {
            Inactive,
            Setting,
            Active,
            Ready,
            Recognizing
        }

        /// <summary>
        ///     音声認識を実行するコンポーネント
        /// </summary>
        public VoiceRecognizer voiceRecognizer;

        public State CurrentState => _currentState.Value;
        public JuliusGrammar CurrentGrammar => _currentGrammar.Value;
        public IObservable<State> OnStateChangedObservable => _currentState.ObserveOnMainThread();
        public IObservable<JuliusGrammar> OnGrammarChangedObservable => _currentGrammar.ObserveOnMainThread();
        public IObservable<bool> OnRecognized => _onRecognized;

        /// <summary>
        ///     音声入力の準備ができているかどうか
        /// </summary>
        private readonly BoolReactiveProperty _isReady = new BoolReactiveProperty(false);

        private readonly Subject<bool> _onRecognized = new Subject<bool>();
        private readonly ReactiveProperty<JuliusGrammar> _currentGrammar = new ReactiveProperty<JuliusGrammar>(null);
        private readonly ReactiveProperty<State> _currentState = new ReactiveProperty<State>(State.Inactive);
        private readonly SemaphoreSlim _clientLock = new SemaphoreSlim(1, 1);

        /// <summary>
        ///     成功するまで、音声認識を実行する
        /// </summary>
        public async UniTask RecognizeUntilSuccess(JuliusGrammar grammar, CancellationToken token = new CancellationToken())
        {
            try
            {
                // ロックの解除を待つ
                await _clientLock.WaitAsync(token);
                if (token.IsCancellationRequested) return;

                // 辞書ファイルをセットする
                await SetGrammarAsync(grammar, token);
                if (token.IsCancellationRequested) return;

                while (true)
                {
                    // 準備完了を待つ
                    await WaitReadyAsync(token);
                    if (token.IsCancellationRequested) return;

                    // WhenAnyで完了しなかったほうのTaskはCancelする必要がある
                    using (var cts = new CancellationTokenSource())
                    using (token.Register(cts.Cancel))
                    {
                        try
                        {
                            // キャンセルもしくは認識完了を待つ
                            var (win, result) =
                                await UniTask.WhenAny(RecognizeAsync(cts.Token), WaitCancelAsync(cts.Token));
                            if (token.IsCancellationRequested || win && result) return;
                        }
                        finally
                        {
                            cts.Cancel();
                        }
                    }
                }
            }
            finally
            {
                _clientLock.Release();
            }
        }

        public void SetReady(bool isReady)
        {
            _isReady.Value = isReady;
        }

        private UniTask SetGrammarAsync(JuliusGrammar grammar, CancellationToken token = new CancellationToken())
        {
            _currentState.Value = State.Setting;
            _currentGrammar.Value = grammar;
            return voiceRecognizer.SetGrammar(grammar, token);
        }

        private UniTask WaitReadyAsync(CancellationToken token = new CancellationToken())
        {
            _currentState.Value = State.Active;
            return _isReady.Where(x => x).AsUnitObservable().First().ToUniTask(cancellationToken: token);
        }

        private UniTask WaitCancelAsync(CancellationToken token = new CancellationToken())
        {
            return _isReady.Where(x => !x).AsUnitObservable().First().ToUniTask(cancellationToken: token)
                .ContinueWith(_ => _currentState.Value = State.Active);
        }

        private UniTask<bool> RecognizeAsync(CancellationToken token = new CancellationToken())
        {
            _currentState.Value = State.Ready;
            return voiceRecognizer.TryRecognize(() => _currentState.Value = State.Recognizing, token: token)
                .ContinueWith(result => {
                    if (token.IsCancellationRequested) return false;
                    _onRecognized.OnNext(result);
                    return result;
                });
        }
    }
}
