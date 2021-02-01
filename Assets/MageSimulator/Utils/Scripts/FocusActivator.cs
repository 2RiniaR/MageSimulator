using UnityEngine;
using UnityEngine.EventSystems;

namespace MageSimulator.Utils.Scripts
{
    public class FocusActivator : MonoBehaviour
    {
        public GameObject target;
        public bool activateOnAwake;

        private void Awake()
        {
            if (activateOnAwake)
                SetSelectingObject();
        }

        public void SetSelectingObject()
        {
            Debug.Log("[FocusActivator] activated: " + target);
            EventSystem.current.SetSelectedGameObject(target);
        }
    }
}
