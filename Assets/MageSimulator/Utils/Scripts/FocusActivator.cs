using UnityEngine;
using UnityEngine.EventSystems;

namespace MageSimulator.Utils.Scripts
{
    public class FocusActivator : MonoBehaviour
    {
        public GameObject target;
        public bool activateOnStart;

        private void Start()
        {
            if (activateOnStart)
                SetSelectingObject();
        }

        public void SetSelectingObject()
        {
            EventSystem.current.SetSelectedGameObject(target);
        }
    }
}
