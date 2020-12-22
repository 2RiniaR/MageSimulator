using UnityEngine;

namespace MageSimulator.Scenes.PriorExplanation.Scripts.Pages
{
    public abstract class Page : MonoBehaviour
    {
        private PriorExplanation _priorExplanation;

        protected virtual void Start()
        {
            _priorExplanation = FindObjectOfType<PriorExplanation>();
        }

        protected void MoveToPrevious()
        {
            _priorExplanation.MoveToPreviousPage();
        }

        protected void MoveToNext()
        {
            _priorExplanation.MoveToNextPage();
        }
    }
}