using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Julius;
using MageSimulator.VoiceRecognition.Envelope;
using MageSimulator.VoiceRecognition.Performance;
using UniRx;
using UnityEngine;

namespace MageSimulator.VoiceRecognition
{
    [CreateAssetMenu(fileName = "New Voice Recognizer", menuName = "Voice/VoiceRecognizer", order = 0)]
    public class VoiceRecognizer : ScriptableObject
    {
        private const string GrammarName = "Recording";

        [Header("文章の解析")]
        public JuliusInstance instance;

        [Header("演技の解析")]
        public PerformanceJudge checker;

        private readonly SemaphoreSlim _clientLock = new SemaphoreSlim(1, 1);

        /// <summary>
        ///     文法を設定する
        /// </summary>
        /// <param name="grammar">文法の定義</param>
        /// <param name="token"></param>
        public async UniTask SetGrammar(JuliusGrammar grammar, CancellationToken token = new CancellationToken())
        {
            await _clientLock.WaitAsync(token);
            try
            {
                await instance.Client.Actions.Terminate.Run();
                if (token.IsCancellationRequested) return;
                await instance.Client.Actions.DeleteGrammar.Run(GrammarName);
                if (token.IsCancellationRequested) return;
                await instance.Client.Actions.AddGrammar.Run(GrammarName, grammar.Grammar);
                if (token.IsCancellationRequested) return;
                await instance.Client.Actions.Resume.Run();
            }
            finally
            {
                _clientLock.Release();
            }
        }

        /// <summary>
        ///     音声認識を行う
        /// </summary>
        /// <param name="onInputStarted">音声入力開始時に呼び出されるコールバック</param>
        /// <param name="onInputFinished">音声入力終了時に呼び出されるコールバック</param>
        /// <param name="token"></param>
        /// <returns>
        ///     音声認識に成功した場合は、録音データ
        ///     失敗した場合は、null
        /// </returns>
        public async UniTask<bool> TryRecognize(Action onInputStarted = null, Action onInputFinished = null,
            CancellationToken token = new CancellationToken())
        {
            await _clientLock.WaitAsync(token);

            try
            {
                // 録音を開始
                await UniTask.SwitchToMainThread(token);
                checker.StartJudgeRecording();

                // Juliusの音声入力開始を待つ
                await WaitInputStart(token);
                if (token.IsCancellationRequested) return false;

                onInputStarted?.Invoke();

                var finishRecordTask = UniTask.RunOnThreadPool(() => WaitInputFinish(token), cancellationToken: token);
                var recognitionTask = UniTask.RunOnThreadPool(() => WaitRecognitionResult(token), cancellationToken: token);

                // Juliusの音声入力終了を待つ
                await finishRecordTask;
                if (token.IsCancellationRequested) return false;

                onInputFinished?.Invoke();

                // 認識の成功・失敗のどちらかを待ち受ける
                var isJuliusRecognitionSucceed = await recognitionTask;
                if (token.IsCancellationRequested) return false;

                return isJuliusRecognitionSucceed && checker.StopJudgeRecording();
            }
            finally
            {
                _clientLock.Release();
                checker.CancelJudgeRecording();
            }
        }

        private UniTask WaitInputStart(CancellationToken token = new CancellationToken())
        {
            return instance.Client.Events.Input.OnStateChanged
                .Where(x => x == InputState.Recording)
                .First()
                .ToUniTask(cancellationToken: token);
        }

        private UniTask WaitInputFinish(CancellationToken token = new CancellationToken())
        {
            return instance.Client.Events.Input.OnStateChanged
                .Where(x => x == InputState.Closed)
                .First()
                .ToUniTask(cancellationToken: token);
        }

        private async UniTask<bool> WaitRecognitionResult(CancellationToken token = new CancellationToken())
        {
            var (win, _, result) = await UniTask.WhenAny(
                instance.Client.Events.Recognize.OnRecognizeFailed.First().ToUniTask(cancellationToken: token),
                instance.Client.Events.Recognize.OnRecognizeSucceed.First().ToUniTask(cancellationToken: token)
            );
            return win == 1 && result.PredictedGramsId != null && result.PredictedGramsId.Count > 0 &&
                   result.PredictedGramsId[0] != 0;
        }
    }
}