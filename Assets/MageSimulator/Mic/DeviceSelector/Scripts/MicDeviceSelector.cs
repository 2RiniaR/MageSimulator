using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.BrowserUI;
using MageSimulator.Mic.Scripts;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Mic.DeviceSelector.Scripts
{
    public class MicDeviceSelector : BrowserComponent
    {
        public Transform listItemParent;
        public MicDeviceSelectorItem listItemPrefab;
        public float refreshDeviceListInterval = 2f;
        public int selectingIndex = 0;
        public InputAction selectAction;
        public IObservable<string> OnDeviceChanged => _onDeviceChanged;

        private float _elapsedTimeFromRefresh = 0f;
        private List<string> _devices = new List<string>();
        private readonly List<MicDeviceSelectorItem> _listItems = new List<MicDeviceSelectorItem>();
        private readonly Subject<string> _onDeviceChanged = new Subject<string>();

        private void OnEnable()
        {
            selectAction.Enable();
            Refresh();
            SetSelectingDevice(selectingIndex);
        }

        private void OnDisable()
        {
            selectAction.Disable();
            selectingIndex = -1;
        }

        private void Update()
        {
            _elapsedTimeFromRefresh += Time.deltaTime;
            if (refreshDeviceListInterval >= _elapsedTimeFromRefresh) return;
            _elapsedTimeFromRefresh = 0f;
            Refresh();
        }

        protected override void Initialize()
        {
            Observable.FromEvent<InputAction.CallbackContext>(
                    h => selectAction.performed += h,
                    h => selectAction.performed -= h)
                .Subscribe(ctx =>
                {
                    if (_devices.Count == 0) return;
                    var value = ctx.ReadValue<float>();
                    if (value > 0)
                        SetSelectingDevice((selectingIndex + 1) % _devices.Count);
                    else if (value < 0)
                        SetSelectingDevice((selectingIndex + (_devices.Count - 1)) % _devices.Count);
                })
                .AddTo(this);

            InitializeChildren();
        }

        public void Refresh()
        {
            var newDevices = Microphone.devices.ToList();
            if (newDevices.SequenceEqual(_devices)) return;

            var selectingDeviceName = GetSelectingDevice();
            _devices = Microphone.devices.ToList();
            selectingIndex = _devices.FindIndex(x => x == selectingDeviceName);

            foreach (var item in _listItems)
                Destroy(item.gameObject);
            _listItems.Clear();

            foreach (var (device, index) in _devices.Select((x, i) => (x, i)))
            {
                var item = Instantiate(listItemPrefab, listItemParent);
                item.Text = device;
                item.SetActive(selectingIndex == index);
                _listItems.Add(item);
            }

            if (_devices.Count > 0 && selectingIndex < 0)
            {
                selectingIndex = 0;
                _listItems[0].SetActive(true);
            }

            var afterSelectingDeviceName = GetSelectingDevice();
            if (selectingDeviceName != afterSelectingDeviceName)
                _onDeviceChanged.OnNext(afterSelectingDeviceName);
        }

        public string GetSelectingDevice()
        {
            return _devices.Count <= selectingIndex || selectingIndex < 0 ? null : _devices[selectingIndex];
        }

        public void SetSelectingDevice(string deviceName)
        {
            SetSelectingDevice(_devices.FindIndex(x => x == deviceName));
        }

        public void SetSelectingDevice(int deviceIndex)
        {
            if (_devices.Count <= deviceIndex || deviceIndex < 0 || selectingIndex == deviceIndex)
                return;

            if (0 <= selectingIndex && selectingIndex < _listItems.Count)
                _listItems[selectingIndex].SetActive(false);
            _listItems[deviceIndex].SetActive(true);
            selectingIndex = deviceIndex;
            _onDeviceChanged.OnNext(GetSelectingDevice());
        }
    }
}
