using UnityEngine;

namespace MageSimulator.VoiceRecognition.Envelope
{
    public class JuliusSetup : MonoBehaviour
    {
        [SerializeField] private JuliusInstance target;

        private void OnEnable()
        {
            target.Setup();
        }

        private void OnDisable()
        {
            target.Stop();
        }
    }
}
