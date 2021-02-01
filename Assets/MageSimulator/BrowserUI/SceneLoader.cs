using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.BrowserUI.Events;
using UnityEngine.SceneManagement;

namespace MageSimulator.BrowserUI
{
    public class SceneLoader : BrowserComponent
    {
        public List<string> loadSceneList;
        public List<string> unloadSceneList;

        protected override void PassEvent(BrowserEvent e)
        {
            if (!(e is SubmitEvent)) PublishEvent(e);
            else LoadScenes().Forget();
        }

        protected UniTask LoadScenes(CancellationToken token = new CancellationToken())
        {
            var loadSceneTasks = loadSceneList.Select(x => SceneManager.LoadSceneAsync(x, LoadSceneMode.Additive).ToUniTask(cancellationToken: token));
            var unloadSceneTasks = unloadSceneList.Select(x => SceneManager.UnloadSceneAsync(x).ToUniTask(cancellationToken: token));
            return UniTask.WhenAll(loadSceneTasks.Concat(unloadSceneTasks));
        }
    }
}