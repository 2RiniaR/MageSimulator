using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MageSimulator.Utils
{
    public class SceneTransitionBehaviour : StateMachineBehaviour
    {
        public enum Timing
        {
            OnEnter,
            OnExit
        }

        [Serializable]
        public struct Transition
        {
            public List<string> loadScenesName;
            public List<string> unloadScenesName;
            public string activateStateName;
            public Timing timing;
        }

        public List<Transition> transitions;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var activeTransitions = transitions.Where(transition =>
                transition.timing == Timing.OnEnter && stateInfo.IsName(transition.activateStateName));
            foreach (var transition in activeTransitions)
                Activate(transition);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var activeTransitions = transitions.Where(transition =>
                transition.timing == Timing.OnExit && stateInfo.IsName(transition.activateStateName));
            foreach (var transition in activeTransitions)
                Activate(transition);
        }

        private static void Activate(Transition transition)
        {
            foreach (var sceneName in transition.loadScenesName)
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            foreach (var sceneName in transition.unloadScenesName)
                SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
