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

        private void Start () {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource == null || Microphone.devices.Length <= 0) return;
            var devName = Microphone.devices[0];
            Microphone.GetDeviceCaps(devName, out var minFreq, out var maxFreq);
            audioSource.clip = Microphone.Start(devName, true, 1, maxFreq);
            audioSource.Play();
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
