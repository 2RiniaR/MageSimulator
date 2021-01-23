using UnityEngine;
using UnityEngine.Events;

namespace MageSimulator.Interactive.Scripts
{
    public class Interactive : MonoBehaviour
    {
        public UnityEvent onMoveToPreviousPage;
        public UnityEvent onMoveToNextPage;

        protected void MoveToPreviousPage()
        {
            onMoveToPreviousPage.Invoke();
        }

        protected void MoveToNextPage()
        {
            onMoveToNextPage.Invoke();
        }
    }
}