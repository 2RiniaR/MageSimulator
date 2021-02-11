using System.Collections.Generic;
using System.Linq;
using MageSimulator.Global.Setting;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MageSimulator.Global.Mic.VolumeMeter.Scripts
{
    public class MicVolumeMeter : MonoBehaviour
    {
        public Image volumeGauge;
        public ApplicationSettings settings;

        [Min(0f)] public float rate = 1f;
        [Min(0f)] public float displayMultiply = 1f;
        public int meanFrame = 1;
        public StringReactiveProperty deviceName;
        public float Volume { get; private set; }
        private AudioSource _audioSource;
        private readonly List<float> _volumeHistory = new List<float>();

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            deviceName.Subscribe(SetDevice).AddTo(this);
            if (settings != null)
            {
                rate = settings.audioInputGain.Value;
                deviceName.Value = settings.audioInputDeviceName.Value;
                settings.audioInputGain.Subscribe(x => rate = x).AddTo(this);
                settings.audioInputDeviceName.Subscribe(x => deviceName.Value = x).AddTo(this);
            }
        }

        private void SetDevice(string device)
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();

            Microphone.GetDeviceCaps(device, out _, out var maxFreq);
            _audioSource.clip = Microphone.Start(device, true, 1, maxFreq);
            _audioSource.Play();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            var currentVolume = Mathf.Clamp01(data.Average(Mathf.Abs) * rate * displayMultiply);
            if (_volumeHistory.Count - meanFrame - 1 > 0)
                _volumeHistory.RemoveRange(0, _volumeHistory.Count - meanFrame - 1);
            _volumeHistory.Add(currentVolume);
            Volume = _volumeHistory.Average();
        }

        private void Update()
        {
            volumeGauge.fillAmount = Volume;
        }
    }
}
