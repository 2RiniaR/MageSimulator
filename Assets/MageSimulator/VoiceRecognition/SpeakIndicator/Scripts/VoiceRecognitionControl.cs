using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Utils.Scripts;
using MageSimulator.VoiceRecognition.Envelope;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

namespace MageSimulator.VoiceRecognition.SpeakIndicator.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class VoiceRecognitionControl : MonoBehaviour
    {
        [Serializable]
        public struct AnimatorTriggerComposite
        {
            public string setupCompletedTriggerName;
            public string readyTriggerName;
            public string canceledTriggerName;
            public string recognitionStartedTriggerName;
            public string recognitionSucceedTriggerName;
            public string recognitionFailedTriggerName;
            public string resetTriggerName;
        }

        /// <summary>
        ///     アニメーションとの連携
        /// </summary>
        public AnimatorTriggerComposite animatorTriggers = new AnimatorTriggerComposite
        {
            setupCompletedTriggerName = "CompleteSetup",
            readyTriggerName = "Ready",
            canceledTriggerName = "Cancel",
            recognitionStartedTriggerName = "StartRecognition",
            recognitionSucceedTriggerName = "SuccessRecognition",
            recognitionFailedTriggerName = "FailRecognition",
            resetTriggerName = "Reset"
        };

        /// <summary>
        ///     認識する文章を表示するテキストコンポーネント
        /// </summary>
        public TMP_Text grammarText;

        /// <summary>
        ///     音声認識を実行するコンポーネント
        /// </summary>
        public VoiceRecognizer voiceRecognizer;

        /// <summary>
        ///     音声入力の準備ができているかどうか
        /// </summary>
        private readonly BoolReactiveProperty _isReady = new BoolReactiveProperty(false);

        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;
        private readonly CancellationTokenSource _internalCancellationTokenSource = new CancellationTokenSource();
        private readonly SemaphoreSlim _clientLock = new SemaphoreSlim(1, 1);

        private void Awake()
        {
            _internalCancellationTokenSource.RegisterRaiseCancelOnDestroy(this);
            _animator = GetComponent<Animator>();
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
            if (_stateMachine == null)
                Debug.LogWarning("Animator に ObservableStateMachineTrigger が存在しません");
        }

        /// <summary>
        ///     成功するまで、音声認識を実行する
        /// </summary>
        public async UniTask<AudioClip> RecognizeUntilSuccess(JuliusGrammar grammar, CancellationToken token = new CancellationToken())
        {
            try
            {
                await _clientLock.WaitAsync(token);
                await UniTask.SwitchToMainThread();

                // 表示するテキストをセットする
                grammarText.text = grammar.display;

                // アニメーショントリガー「リセット」を発火する
                RunOnMainThread(OnReset, token);

                // 辞書ファイルをセットする
                await voiceRecognizer.SetGrammar(grammar, token);
                if (token.IsCancellationRequested || _internalCancellationTokenSource.IsCancellationRequested)
                    return null;

                // アニメーショントリガー「録音準備完了」を発火する
                RunOnMainThread(OnSetupCompleted, token);

                // 認識成功するまで、音声認識を繰り返す
                return await RecognizeUntilSuccess(token);
            }
            finally
            {
                _clientLock.Release();
            }
        }

        /// <summary>
        ///     成功するまで、音声認識を実行する
        /// </summary>
        private async UniTask<AudioClip> RecognizeUntilSuccess(CancellationToken token = new CancellationToken())
        {
            while (true)
            {
                // 「外部コンポーネントやユーザーから、音声入力の準備ができた通知が送られる」まで待機
                await _isReady.Where(x => x).First().ToUniTask(cancellationToken: token);
                if (token.IsCancellationRequested || _internalCancellationTokenSource.IsCancellationRequested)
                    return null;

                RunOnMainThread(OnReady, token);
                var cancel = new CancellationTokenSource();
                var onCancelDisposable = new CompositeDisposable
                {
                    cancel,
                    token.Register(() => cancel.Cancel()),
                    _internalCancellationTokenSource.Token.Register(() => cancel.Cancel())
                };

                // 「外部コンポーネントやユーザーから、音声入力をキャンセルする通知が送られる」まで待機するUniTask
                var cancelActionTask = _isReady.Where(x => !x).First().ToUniTask(cancellationToken: cancel.Token);

                // 「音声入力がされて、音声認識が成功または失敗する」まで待機するUniTask
                var recordTask = voiceRecognizer.TryRecognize(() => RunOnMainThread(OnRecognitionStarted, cancel.Token),
                    token: cancel.Token);

                // 上記2つのどちらかが先に完了するまで、待機する
                var (win, _, result) = await UniTask.WhenAny(cancelActionTask, recordTask);
                if (token.IsCancellationRequested || _internalCancellationTokenSource.IsCancellationRequested)
                {
                    RunOnMainThread(OnCanceled, token);
                    return null;
                }

                // 終了していないほうのUniTaskをキャンセルする
                cancel.Cancel();
                onCancelDisposable.Dispose();

                switch (win)
                {
                    case 0:
                        // 外部からキャンセルされた場合、継続する
                        RunOnMainThread(OnCanceled, token);
                        break;

                    case 1 when result != null:
                        // 認識に成功した場合、録音を返す
                        RunOnMainThread(OnRecognitionSucceed, CancellationToken.None);
                        return result;

                    case 1:
                        // 認識に失敗した場合、継続する
                        RunOnMainThread(OnRecognitionFailed, token);
                        break;
                }
            }
        }

        public void SetReady(bool isReady)
        {
            _isReady.Value = isReady;
        }

        private static void RunOnMainThread(Action action, CancellationToken token = new CancellationToken())
        {
            UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.SwitchToMainThread(token);
                action();
            }, cancellationToken: token).Forget();
        }

        private void OnReset()
        {
            if (_animator == null) return;
            _animator.SetTrigger(animatorTriggers.resetTriggerName);
        }

        private void OnSetupCompleted()
        {
            if (_animator == null) return;
            _animator.SetTrigger(animatorTriggers.setupCompletedTriggerName);
        }

        private void OnReady()
        {
            if (_animator == null) return;
            _animator.SetTrigger(animatorTriggers.readyTriggerName);
        }

        private void OnCanceled()
        {
            if (_animator == null) return;
            _animator.ResetTrigger(animatorTriggers.readyTriggerName);
            _animator.SetTrigger(animatorTriggers.canceledTriggerName);
        }

        private void OnRecognitionStarted()
        {
            if (_animator == null) return;
            _animator.SetTrigger(animatorTriggers.recognitionStartedTriggerName);
        }

        private void OnRecognitionFailed()
        {
            if (_animator == null) return;
            _animator.ResetTrigger(animatorTriggers.readyTriggerName);
            _animator.SetTrigger(animatorTriggers.recognitionFailedTriggerName);
        }

        private void OnRecognitionSucceed()
        {
            if (_animator == null) return;
            _animator.SetTrigger(animatorTriggers.recognitionSucceedTriggerName);
        }
    }
}
