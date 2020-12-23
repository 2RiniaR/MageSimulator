using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Scenes.PriorExplanation.Scripts
{
    public class PriorExplanation : MonoBehaviour
    {
        public Animator animator;
        public string firstPageStateName;
        public string lastPageStateName;
        public string exitSceneAnimatorTriggerName;
        private Interactive.Interactive _interactive;

        private void Start()
        {
            _interactive = FindObjectOfType<Interactive.Interactive>();
        }
    }
}
