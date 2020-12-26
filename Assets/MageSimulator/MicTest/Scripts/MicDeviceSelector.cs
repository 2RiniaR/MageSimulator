using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MageSimulator.MicTest.Scripts
{
    public class MicDeviceSelector : MonoBehaviour
    {
        public Transform listItemParent;
        public MicDeviceSelectorItem listItemPrefab;
        public float refreshDeviceListInterval = 2f;
        public int selectingIndex = 0;
        public InputAction selectAction;
        public UnityEvent<string> onDeviceChanged;

        private float _elapsedTimeFromRefresh = 0f;
        private List<string> _devices = new List<string>();
        private readonly List<MicDeviceSelectorItem> _listItems = new List<MicDeviceSelectorItem>();

        private void OnEnable()
        {
            selectAction.Enable();
            selectAction.performed += OnSelect;
            Refresh();
            SetSelectingDevice(selectingIndex);
        }

        private void OnDisable()
        {
            selectAction.performed -= OnSelect;
            selectAction.Disable();
            selectingIndex = -1;
        }

        private void Start()
        {
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
                onDeviceChanged.Invoke(afterSelectingDeviceName);
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            if (_devices.Count == 0) return;

            var value = ctx.ReadValue<float>();
            int deviceIndex;
            if (value < 0)
                deviceIndex = (selectingIndex + (_devices.Count - 1)) % _devices.Count;
            else
                deviceIndex = (selectingIndex + 1) % _devices.Count;

            SetSelectingDevice(deviceIndex);
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
            onDeviceChanged.Invoke(GetSelectingDevice());
        }
    }
}
