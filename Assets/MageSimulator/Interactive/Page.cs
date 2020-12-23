using MageSimulator.Scenes.PriorExplanation.Scripts;
using UnityEngine;

namespace MageSimulator.Interactive
{
    public abstract class Page : MonoBehaviour
    {
        private Interactive _interactive;

        protected virtual void Start()
        {
            _interactive = GetComponentInParent<Interactive>();
        }

        protected void MoveToPrevious()
        {
            _interactive.OnMoveToPreviousPage.Invoke();
        }

        protected void MoveToNext()
        {
            _interactive.OnMoveToNextPage.Invoke();
        }
    }
}