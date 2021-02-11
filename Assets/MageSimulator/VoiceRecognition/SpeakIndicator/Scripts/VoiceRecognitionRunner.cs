using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Effects;
using MageSimulator.Combo.Events;
using MageSimulator.VoiceRecognition.Envelope;
using UniRx;
using UnityEngine;

namespace MageSimulator.VoiceRecognition.SpeakIndicator.Scripts
{
    public class VoiceRecognitionRunner : ComboEffect
    {
        public JuliusGrammar grammar;
        public IObservable<Unit> OnSucceed => _onSucceed;
        private VoiceRecognitionControl _control;
        private readonly Subject<Unit> _onSucceed = new Subject<Unit>();
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _control = FindObjectOfType<VoiceRecognitionControl>();
            if (_control == null)
                Debug.LogWarning("VoiceRecognitionControl が存在しません。");
        }

        protected override void Initialize()
        {
            Activate();
        }

        protected override void PassEvent(ComboEvent e)
        {
            if (e is FinishPageEvent) Deactivate();
        }

        public void Activate()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.RegisterRaiseCancelOnDestroy(this);
            _control.SetReady(true);
            LoopVoiceRecognition(_cancellationTokenSource.Token).Forget();
        }

        public void Deactivate()
        {
            _control.SetReady(false);
            _cancellationTokenSource?.Cancel();
        }

        private async UniTask LoopVoiceRecognition(CancellationToken token = new CancellationToken())
        {
            while (true)
            {
                await _control.RecognizeUntilSuccess(grammar, token);
                if (token.IsCancellationRequested) return;
                _onSucceed.OnNext(Unit.Default);
            }
        }
    }
}