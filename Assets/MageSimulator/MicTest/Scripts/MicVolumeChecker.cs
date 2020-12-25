using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MageSimulator.MicTest.Scripts
{
    public class MicVolumeChecker : MonoBehaviour
    {
        public Image volumeGauge;
        public float Volume { get; private set; }
        private AudioSource _audioSource;

        private void Start () {
            _audioSource = GetComponent<AudioSource>();
        }

        public void SetDevice(string deviceName)
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();

            Microphone.GetDeviceCaps(deviceName, out var minFreq, out var maxFreq);
            _audioSource.clip = Microphone.Start(deviceName, true, 1, maxFreq);
            _audioSource.Play();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            Volume = data.Average(Mathf.Abs);
        }

        private void Update()
        {
            volumeGauge.fillAmount = Volume;
        }
    }
}
