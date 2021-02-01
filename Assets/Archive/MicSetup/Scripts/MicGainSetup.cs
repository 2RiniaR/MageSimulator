using MageSimulator.Config.Scripts;
using MageSimulator.Mic.Scripts;
using MageSimulator.Mic.VolumeMeter.Scripts;
using UnityEngine;

namespace MageSimulator.Scenes.MicSetup.Scripts
{
    public class MicGainSetup : MonoBehaviour
    {
        public MicGainSetter gainSetter;
        public MicVolumeChecker volumeChecker;
        private ApplicationSettings _applicationSettings;

        private void Awake()
        {
            _applicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
        }

        private void OnEnable()
        {
            gainSetter.onRateChanged.AddListener(OnGainRateChanged);
            volumeChecker.deviceName.Value = _applicationSettings.audioInputDeviceName;
            volumeChecker.rate = _applicationSettings.audioInputGain;
        }

        private void OnDisable()
        {
            gainSetter.onRateChanged.RemoveListener(OnGainRateChanged);
        }

        private void OnGainRateChanged(float rate)
        {
            _applicationSettings.audioInputGain = rate;
            volumeChecker.rate = rate;
        }
    }
}
