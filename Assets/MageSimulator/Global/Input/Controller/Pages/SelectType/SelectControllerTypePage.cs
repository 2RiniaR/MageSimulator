using MageSimulator.Global.Input.Controller.Wiimote;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WiimoteApi;

namespace MageSimulator.Global.Input.Controller.Pages.SelectType
{
    public class SelectControllerTypePage : MonoBehaviour
    {
        public Button wiimoteButton;
        public float refreshControllerTime;
        private float _elapsedTime = 0f;

        private void Start()
        {
            RefreshController();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < refreshControllerTime) return;
            _elapsedTime = 0f;
            RefreshController();
        }

        private void RefreshController()
        {
            var wiimoteDevice = InputSystem.GetDevice<WiimoteDevice>();
            wiimoteButton.interactable = wiimoteDevice != null &&
                                         wiimoteDevice.Wiimote.current_ext == ExtensionController.MOTIONPLUS;
        }
    }
}