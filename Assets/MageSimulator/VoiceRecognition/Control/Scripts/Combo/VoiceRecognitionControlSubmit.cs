using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Components;
using MageSimulator.Global.Setting;
using MageSimulator.VoiceRecognition.Envelope;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.VoiceRecognition.Control.Scripts.Combo
{
    public class VoiceRecognitionControlSubmit : Submit
    {
        public JuliusGrammar grammar;
        public InputAction readyAction;
        public VoiceRecognitionControl control;

        private void OnEnable()
        {
            readyAction.Enable();
        }

        private void OnDisable()
        {
            readyAction.Disable();
        }

        protected override void Start()
        {
            Observable.FromEvent<InputAction.CallbackContext>(h => readyAction.started += h,
                    h => readyAction.started -= h)
                .Subscribe(_ => control.SetReady(true)).AddTo(this);

            Observable.FromEvent<InputAction.CallbackContext>(h => readyAction.canceled += h,
                    h => readyAction.canceled -= h)
                .Subscribe(_ => control.SetReady(false)).AddTo(this);

            base.Start();
        }

        protected override void Initialize()
        {
            control.RecognizeUntilSuccess(grammar, CancellationTokenSource.Token)
                .ContinueWith(Publish)
                .Forget();

            base.Initialize();
        }
    }
}