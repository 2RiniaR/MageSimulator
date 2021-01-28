using System;
using UniRx;
using UnityEngine;

namespace MageSimulator.Mic.Recorder
{
    [CreateAssetMenu(fileName = "New Mic Recorder", menuName = "Mic/Recorder", order = 0)]
    public class MicRecorder : ScriptableObject
    {
        public bool IsRecording => _isRecording.Value;
        public AudioClip LastRecodingClip { get; private set; }
        public float[] LastRecordingData { get; private set; }

        [Min(0f)] public float rate;
        [Min(0f)] public int maxDuration;
        public string deviceName;
        public IObservable<AudioClip> OnRecordFinishObservable => _onRecordFinishObservable;

        private AudioClip _audioClip;
        private readonly BoolReactiveProperty _isRecording = new BoolReactiveProperty(false);
        private readonly Subject<AudioClip> _onRecordFinishObservable = new Subject<AudioClip>();

        public void StartRecording()
        {
            Microphone.GetDeviceCaps(deviceName, out _, out var maxFreq);
            _audioClip = Microphone.Start(deviceName, false, maxDuration, maxFreq);
            _isRecording.Value = true;
        }

        public void StopRecording()
        {
            // 録音を停止する
            var position = Microphone.GetPosition(deviceName);
            Microphone.End(deviceName);

            // 録音データを取り出す
            // _onRecordFinishObservable.OnNext(_audioClip);
            var soundData = new float[_audioClip.samples * _audioClip.channels];
            _audioClip.GetData(soundData, 0);

            // 録音データから余白部分を切り取り、保持する
            LastRecordingData = new float[position * _audioClip.channels];
            for (var i = 0; i < LastRecordingData.Length; i++)
                LastRecordingData[i] = soundData[i] * rate;
            LastRecodingClip =
                AudioClip.Create(_audioClip.name, position, _audioClip.channels, _audioClip.frequency, false);
            LastRecodingClip.SetData(LastRecordingData, 0);
            _onRecordFinishObservable.OnNext(LastRecodingClip);

            _isRecording.Value = false;
        }
    }
}