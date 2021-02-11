﻿using MageSimulator.Combo.Components;
using MageSimulator.Global.Input.Controller.Wiimote.MotionDetector;
using MageSimulator.VoiceRecognition.SpeakIndicator.Scripts;
using UniRx;
using UnityEngine;

namespace MageSimulator.Scenes.Magic.Scripts.Combo
{
    [RequireComponent(typeof(MotionDetector))]
    public class MagicActionSubmit : Submit
    {
        public float voiceRecognitionMarginTime;
        public float motionDetectionMarginTime;

        private VoiceRecognitionRunner _voiceRecognizer;
        private MotionDetector _motionDetector;
        private float _speakActiveTime = 0f;
        private float _motionActiveTime = 0f;
        private readonly BoolReactiveProperty _inCondition = new BoolReactiveProperty(false);

        protected override void Start()
        {
            _voiceRecognizer = GetComponentInParent<VoiceRecognitionRunner>();
            _motionDetector = GetComponent<MotionDetector>();

            _voiceRecognizer.OnSucceed
                .Subscribe(_ => _speakActiveTime = Mathf.Max(voiceRecognitionMarginTime, float.Epsilon))
                .AddTo(this);

            _motionDetector.OnDetect
                .Subscribe(_ => _motionActiveTime = Mathf.Max(motionDetectionMarginTime, float.Epsilon))
                .AddTo(this);

            _inCondition
                .Where(x => x)
                .Subscribe(_ => Publish())
                .AddTo(this);

            base.Start();
        }

        private void Update()
        {
            _inCondition.Value = _speakActiveTime > 0f && _motionActiveTime > 0f;
            _speakActiveTime = Mathf.Max(0f, _speakActiveTime - Time.deltaTime);
            _motionActiveTime = Mathf.Max(0f, _motionActiveTime - Time.deltaTime);
        }
    }
}