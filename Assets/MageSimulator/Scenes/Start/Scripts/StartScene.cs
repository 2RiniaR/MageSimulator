using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Scenes.Start.Scripts
{
    public class StartScene : MonoBehaviour
    {
        public Animator animator;
        public string enterAnimatorTriggerName;
        public InputAction enterAction;

        private void OnEnable()
        {
            enterAction.Enable();
            enterAction.performed += OnEnter;
        }

        private void OnDisable()
        {
            enterAction.performed -= OnEnter;
            enterAction.Disable();
        }

        private void OnEnter(InputAction.CallbackContext ctx)
        {
            animator.SetTrigger(enterAnimatorTriggerName);
        }
    }
}
