using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MageSimulator.Utils.Scripts
{
    public class FocusDebugger : MonoBehaviour
    {
        private void Awake()
        {
            EventSystem.current.currentSelectedGameObject.ObserveEveryValueChanged(x => x)
                .Subscribe(x => Debug.Log("Focus: " + x)).AddTo(this);
        }
    }
}