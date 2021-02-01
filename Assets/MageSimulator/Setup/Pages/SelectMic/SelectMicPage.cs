using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.BrowserUI;
using MageSimulator.BrowserUI.Events;
using MageSimulator.Config.Scripts;
using MageSimulator.Mic.DeviceSelector.Scripts;
using MageSimulator.Mic.VolumeMeter.Scripts;
using UniRx;

namespace MageSimulator.Setup.Pages.SelectMic
{
    public class SelectMicPage : BrowserComponent
    {
        public ApplicationSettings settings;
        public MicDeviceSelector deviceSelector;
        public MicVolumeChecker volumeChecker;

        protected override void Start()
        {
            deviceSelector.SetSelectingDevice(settings.audioInputDeviceName);
            volumeChecker.deviceName.Value = deviceSelector.GetSelectingDevice();
            deviceSelector.OnDeviceChanged.Subscribe(x => volumeChecker.deviceName.Value = x).AddTo(volumeChecker);
            base.Start();
        }

        protected override void PassEvent(BrowserEvent e)
        {
            if (e is SubmitEvent)
                settings.audioInputDeviceName = deviceSelector.GetSelectingDevice();

            PublishEvent(e);
        }
    }
}