using System;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Components;
using MageSimulator.Global.Setting;
using MageSimulator.VoiceRecognition.Envelope;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.VoiceRecognition.SpeakIndicator.Scripts.Combo
{
    public class SpeakSubmit : Submit
    {
        public JuliusGrammar grammar;
        public InputAction readyAction;
        private VoiceRecognitionControl _voiceRecognitionControl;
        private ApplicationSettings _settings;

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
            _voiceRecognitionControl = FindObjectOfType<VoiceRecognitionControl>();
            _settings = Resources.Load<ApplicationSettings>("ApplicationSettings");

            Observable.FromEvent<InputAction.CallbackContext>(h => readyAction.started += h,
                    h => readyAction.started -= h)
                .Do(_ => Debug.Log("start"))
                .Subscribe(_ => _voiceRecognitionControl.SetReady(true)).AddTo(this);

            Observable.FromEvent<InputAction.CallbackContext>(h => readyAction.canceled += h,
                    h => readyAction.canceled -= h)
                .Do(_ => Debug.Log("cancel"))
                .Subscribe(_ => _voiceRecognitionControl.SetReady(false)).AddTo(this);

            base.Start();
        }

        protected override void Initialize()
        {
            UniTask.RunOnThreadPool(async () =>
            {
                var recording = await _voiceRecognitionControl.RecognizeUntilSuccess(grammar, CancellationTokenSource.Token);
                WavUtility.SaveAsFile(recording, _settings.recordingSaveDirectoryPath + "/" + DateTime.Now.ToString("yyMMdd_HHmmss_FFF") + ".wav");
                Publish();
            }, cancellationToken: CancellationTokenSource.Token);

            base.Initialize();
        }
    }
}