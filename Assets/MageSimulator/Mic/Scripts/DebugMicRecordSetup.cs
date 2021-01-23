using UnityEngine;

namespace MageSimulator.Mic.Scripts
{
    public class DebugMicRecordSetup : MonoBehaviour
    {
        [SerializeField]
        private MicRecorder target;

        public float rate;
        public int deviceNumber;

        private void Start()
        {
            if (deviceNumber < Microphone.devices.Length)
                target.deviceName = Microphone.devices[deviceNumber];
            target.rate = rate;
        }
    }
}
