using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Julius;
using MageSimulator.Mic.Scripts;
using UniRx;
using UnityEngine;

namespace MageSimulator.DataCollection.Scripts
{
    [CreateAssetMenu(fileName = "New Data Collector", menuName = "MageSimulator/DataCollector", order = 0)]
    public class DataCollector : ScriptableObject
    {
        public struct RecordCallbacks
        {
            public Action OnStartInput;
            public Action OnFinishInput;
        }

        public enum Step
        {
            Inactive,
            Setting,
            WaitingAudioInput,
            ReceivingAudioInput,
            WaitingRecognizeResult
        }

        private const string GrammarName = "Recording";

        [Header("包絡成分の解析")]
        public JuliusInstance instance;

        [Header("録音")]
        public MicRecorder recorder;

        [Header("微細構造成分の解析")]
        public AvailabilityChecker checker;

        public Step CurrentStep { get; private set; } = Step.Inactive;
        public IObservable<Unit> OnInputStarted => _onInputStarted;
        public IObservable<Unit> OnInputFinished => _onInputFinished;
        public IObservable<Unit> OnSucceed => _onSucceed;
        public IObservable<Unit> OnRetry => _onRetry;

        private readonly Subject<Unit> _onInputStarted = new Subject<Unit>();
        private readonly Subject<Unit> _onInputFinished = new Subject<Unit>();
        private readonly Subject<Unit> _onSucceed = new Subject<Unit>();
        private readonly Subject<Unit> _onRetry = new Subject<Unit>();
        private readonly SemaphoreSlim _clientLock = new SemaphoreSlim(1, 1);

        public async UniTask SetGrammar(JuliusGrammar grammar, CancellationToken token = new CancellationToken())
        {
            await _clientLock.WaitAsync(token);
            try
            {
                CurrentStep = Step.Setting;
                await instance.Client.Actions.Terminate.Run();
                if (token.IsCancellationRequested) return;
                await instance.Client.Actions.DeleteGrammar.Run(GrammarName);
                if (token.IsCancellationRequested) return;
                await instance.Client.Actions.AddGrammar.Run(GrammarName, grammar.Grammar);
                if (token.IsCancellationRequested) return;
                await instance.Client.Actions.Resume.Run();
                CurrentStep = Step.Inactive;
            }
            finally
            {
                _clientLock.Release();
            }
        }

        public async UniTask<AudioClip> Record(CancellationToken token = new CancellationToken())
        {
            await _clientLock.WaitAsync(token);

            try
            {
                while (true)
                {
                    // 録音を開始
                    recorder.StartRecording();

                    // Juliusの音声入力開始を待つ
                    CurrentStep = Step.WaitingAudioInput;
                    await WaitInputStart(token);

                    if (token.IsCancellationRequested) return null;

                    CurrentStep = Step.ReceivingAudioInput;
                    _onInputStarted.OnNext(Unit.Default);

                    var finishRecordTask = UniTask.RunOnThreadPool(() => WaitInputFinish(token), cancellationToken: token);
                    var recognitionTask = UniTask.RunOnThreadPool(() => WaitRecognitionResult(token), cancellationToken: token);

                    // Juliusの音声入力終了を待つ
                    await finishRecordTask;

                    if (token.IsCancellationRequested) return null;

                    CurrentStep = Step.WaitingRecognizeResult;
                    _onInputFinished.OnNext(Unit.Default);

                    // 録音を終了・保持
                    await UniTask.SwitchToMainThread();
                    recorder.StopRecording();
                    var recording = recorder.LastRecodingClip;

                    // 認識の成功・失敗のどちらかを待ち受ける
                    var isJuliusRecognitionSucceed = await recognitionTask;
                    if (token.IsCancellationRequested) return null;

                    // 認識成功ならば録音を返し、失敗なら繰り返す
                    if (isJuliusRecognitionSucceed && checker.CheckAvailability(recording))
                    {
                        CurrentStep = Step.Inactive;
                        _onSucceed.OnNext(Unit.Default);
                        return recording;
                    }

                    _onRetry.OnNext(Unit.Default);
                }
            }
            finally
            {
                _clientLock.Release();
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
                   result.PredictedGramsId[0] == 1;
        }
    }
}