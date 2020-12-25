using UnityEngine;

namespace MageSimulator.Config.Scripts
{
    [CreateAssetMenu(fileName = "New Application Settings", menuName = "MageSimulator/Settings")]
    public class ApplicationSettings : ScriptableObject
    {
        public bool allowRecording;
        public string audioInputDeviceName;
        public float audioInputGain;
    }
}
