using MageSimulator.Config.Scripts;
using MageSimulator.Mic.DeviceSelector.Scripts;
using MageSimulator.Mic.VolumeMeter.Scripts;
using UnityEngine;
using UniRx;

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
            deviceSelector.OnDeviceChanged.Subscribe(OnDeviceSelectChanged).AddTo(this);
            deviceSelector.SetSelectingDevice(_applicationSettings.audioInputDeviceName);
            volumeChecker.rate = _applicationSettings.audioInputGain;
            volumeChecker.deviceName.Value = _applicationSettings.audioInputDeviceName;
        }

        private void OnDeviceSelectChanged(string deviceName)
        {
            _applicationSettings.audioInputDeviceName = deviceName;
            volumeChecker.deviceName.Value = _applicationSettings.audioInputDeviceName;
        }
    }
}