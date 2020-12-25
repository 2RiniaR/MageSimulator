using System.Collections.Generic;
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

        public List<string> loadScenesName;
        public List<string> unloadScenesName;

        public string activateStateName;

        public Timing timing;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (timing == Timing.OnEnter && stateInfo.IsName(activateStateName))
                Activate();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (timing == Timing.OnExit && stateInfo.IsName(activateStateName))
                Activate();
        }

        private void Activate()
        {
            foreach (var sceneName in loadScenesName)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            foreach (var sceneName in unloadScenesName)
            {
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }
    }
}
