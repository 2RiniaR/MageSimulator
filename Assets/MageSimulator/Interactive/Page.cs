using MageSimulator.Scenes.PriorExplanation.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace MageSimulator.Interactive
{
    public abstract class Page : MonoBehaviour
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
            _interactive.OnMoveToPreviousPage.Invoke();
        }

        protected void MoveToNext()
        {
            onMoveToNext.Invoke();
            _interactive.OnMoveToNextPage.Invoke();
        }
    }
}