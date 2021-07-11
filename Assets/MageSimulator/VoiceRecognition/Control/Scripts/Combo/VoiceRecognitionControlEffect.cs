using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Effects;
using MageSimulator.Combo.Events;
using MageSimulator.VoiceRecognition.Envelope;
using UniRx;
using UnityEngine;

namespace MageSimulator.VoiceRecognition.Control.Scripts.Combo
{
    public class VoiceRecognitionControlEffect : ComboEffect
    {
        public JuliusGrammar grammar;
        public VoiceRecognitionControl control;
        public IObservable<Unit> OnSucceed => _onSucceed;

        private readonly Subject<Unit> _onSucceed = new Subject<Unit>();
        private CancellationTokenSource _cancellationTokenSource;

        protected override void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.RegisterRaiseCancelOnDestroy(this);
            control.OnStateChangedObservable.Subscribe(x => Debug.Log("state: " + x)).AddTo(this);
            control.SetReady(true);
            LoopVoiceRecognition(_cancellationTokenSource.Token).Forget();
        }

        protected override void PassEvent(ComboEvent e)
        {
            if (!(e is FinishPageEvent)) return;
            control.SetReady(false);
            _cancellationTokenSource?.Cancel();
        }

        private async UniTask LoopVoiceRecognition(CancellationToken token = new CancellationToken())
        {
            while (true)
            {
                await control.RecognizeUntilSuccess(grammar, token).ContinueWith(() => _onSucceed.OnNext(Unit.Default));
                if (token.IsCancellationRequested) return;
            }
        }
    }
}