using UnityEngine;

namespace MageSimulator.DataCollection.Scripts
{
    [CreateAssetMenu(fileName = "New Availability Checker", menuName = "Sound/AvailabilityChecker", order = 0)]
    public class AvailabilityChecker : ScriptableObject
    {
        [Range(0f, 1f)]
        public float threshold;

        public AudioClip baseAudioClip;

        public bool CheckAvailability(AudioClip clip)
        {
            return 0.5 < threshold;
        }
    }
}