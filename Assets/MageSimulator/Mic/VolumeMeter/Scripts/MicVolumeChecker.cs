using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MageSimulator.Mic.VolumeMeter.Scripts
{
    public class MicVolumeChecker : MonoBehaviour
    {
        public Image volumeGauge;

        [Min(0f)] public float rate = 1f;
        public StringReactiveProperty deviceName;
        public float Volume { get; private set; }
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            deviceName.Subscribe(SetDevice).AddTo(this);
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
            Volume = Mathf.Clamp01(data.Average(Mathf.Abs) * rate);
        }

        private void Update()
        {
            volumeGauge.fillAmount = Volume;
        }
    }
}
