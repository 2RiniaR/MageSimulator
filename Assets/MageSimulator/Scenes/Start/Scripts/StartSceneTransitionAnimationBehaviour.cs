using UnityEngine;
using UnityEngine.SceneManagement;

namespace MageSimulator.Scenes.Start.Scripts
{
    public class StartSceneTransitionAnimationBehaviour : StateMachineBehaviour
    {
        public string exitStateName;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!stateInfo.IsName(exitStateName)) return;
            SceneManager.LoadSceneAsync("事前説明", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("スタート画面", UnloadSceneOptions.None);
        }
    }
}
