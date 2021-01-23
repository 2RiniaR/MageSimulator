using MageSimulator.Config.Scripts;
using MageSimulator.Mic.Scripts;
using UnityEngine;

namespace MageSimulator.Scenes.MicSetup.Scripts
{
    public class MicDeviceSetup : MonoBehaviour
    {
        public MicDeviceSelector deviceSelector;
        public MicVolumeChecker volumeChecker;
        private ApplicationSettings _applicationSettings;

        private void Awake()
        {
            _applicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
        }

        private void OnEnable()
        {
            deviceSelector.onDeviceChanged.AddListener(OnDeviceSelectChanged);
            deviceSelector.SetSelectingDevice(_applicationSettings.audioInputDeviceName);
            volumeChecker.rate = _applicationSettings.audioInputGain;
            volumeChecker.deviceName = _applicationSettings.audioInputDeviceName;
        }

        private void OnDisable()
        {
            deviceSelector.onDeviceChanged.RemoveListener(OnDeviceSelectChanged);
        }

        private void OnDeviceSelectChanged(string deviceName)
        {
            _applicationSettings.audioInputDeviceName = deviceName;
            volumeChecker.deviceName = _applicationSettings.audioInputDeviceName;
        }
    }
}