using UnityEngine;
using UnityEngine.Events;

namespace MageSimulator.Interactive.Scripts
{
    public class Page : MonoBehaviour
    {
        private Interactive _interactive;
        public UnityEvent onMoveToPrevious;
        public UnityEvent onMoveToNext;

        protected virtual void Start()
        {
            _interactive = GetComponentInParent<Interactive>();
        }

        protected void MoveToPrevious()
        {
            onMoveToPrevious.Invoke();
            if (_interactive != null)
                _interactive.onMoveToPreviousPage.Invoke();
        }

        protected void MoveToNext()
        {
            onMoveToNext.Invoke();;
            if (_interactive != null)
                _interactive.onMoveToNextPage.Invoke();
        }
    }
}