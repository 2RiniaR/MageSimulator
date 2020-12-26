using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MageSimulator.MicTest.Scripts
{
    public class MicVolumeChecker : MonoBehaviour
    {
        public Image volumeGauge;

        [Min(0f)] public float rate;
        public string deviceName;
        public float Volume { get; private set; }
        private AudioSource _audioSource;
        private string _previousDeviceName = null;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void SetDevice()
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();

            Microphone.GetDeviceCaps(deviceName, out _, out var maxFreq);
            _audioSource.clip = Microphone.Start(deviceName, true, 1, maxFreq);
            _audioSource.Play();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            Volume = Mathf.Clamp01(data.Average(Mathf.Abs) * rate);
        }

        private void Update()
        {
            if (_previousDeviceName != deviceName)
            {
                SetDevice();
                _previousDeviceName = deviceName;
            }
            volumeGauge.fillAmount = Volume;
        }
    }
}
