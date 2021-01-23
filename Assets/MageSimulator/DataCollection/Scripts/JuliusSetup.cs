using UnityEngine;

namespace MageSimulator.DataCollection.Scripts
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
