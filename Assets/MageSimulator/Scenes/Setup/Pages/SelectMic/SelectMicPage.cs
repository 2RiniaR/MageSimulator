using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.BrowserUI;
using MageSimulator.Config.Scripts;
using MageSimulator.Mic.DeviceSelector.Scripts;
using MageSimulator.Mic.VolumeMeter.Scripts;
using UniRx;

namespace MageSimulator.Scenes.Setup.Pages.SelectMic
{
    public class SelectMicPage : BrowserComponent
    {
        public ApplicationSettings settings;
        public MicDeviceSelector deviceSelector;
        public MicVolumeChecker volumeChecker;

        private void Start()
        {
            deviceSelector.OnDeviceChanged.Subscribe(x => volumeChecker.deviceName = x).AddTo(volumeChecker);
            deviceSelector.SetSelectingDevice(settings.audioInputDeviceName);
        }

        protected override UniTask OnClosePage(Link source, CancellationToken token = new CancellationToken())
        {
            settings.audioInputDeviceName = deviceSelector.GetSelectingDevice();
            return UniTask.CompletedTask;
        }
    }
}