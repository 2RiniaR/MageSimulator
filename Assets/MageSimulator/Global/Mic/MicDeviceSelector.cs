using System;
using System.Collections.Generic;
using System.Linq;
using MageSimulator.Combo.Components;
using MageSimulator.Global.Setting;
using TMPro;
using UniRx;
using UnityEngine;

namespace MageSimulator.Global.Mic
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class MicDeviceSelector : MonoBehaviour
    {
        public ApplicationSettings settings;

        public float refreshDeviceListInterval = 2f;
        public IObservable<string> OnDeviceChanged => _onDeviceChanged;
        private TMP_Dropdown _dropdown;

        private float _elapsedTimeFromRefresh = 0f;
        private List<string> _devices = new List<string>();
        private readonly Subject<string> _onDeviceChanged = new Subject<string>();

        private void Start()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            Refresh();
            var fromSetting = _dropdown.options.FindIndex(x => x.text == settings.audioInputDeviceName.Value);
            if (fromSetting > 0) _dropdown.value = fromSetting;

            _dropdown.onValueChanged.AsObservable()
                .Subscribe(x => settings.audioInputDeviceName.Value = _dropdown.options[x].text)
                .AddTo(this);
        }

        private void Update()
        {
            _elapsedTimeFromRefresh += Time.deltaTime;
            if (refreshDeviceListInterval >= _elapsedTimeFromRefresh) return;
            _elapsedTimeFromRefresh = 0f;
            Refresh();
        }

        public void Refresh()
        {
            var newDevices = Microphone.devices.ToList();
            if (newDevices.SequenceEqual(_devices)) return;
            _devices = Microphone.devices.ToList();

            _dropdown.ClearOptions();
            _dropdown.AddOptions(_devices);
            _dropdown.RefreshShownValue();
        }
    }
}
