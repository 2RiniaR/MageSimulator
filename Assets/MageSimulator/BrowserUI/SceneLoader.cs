using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace MageSimulator.BrowserUI
{
    public abstract class SceneLoader : BrowserComponent
    {
        public List<string> loadSceneList;
        public List<string> unloadSceneList;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelWith(this);
        }

        protected UniTask LoadScenes(CancellationToken token = new CancellationToken())
        {
            var loadSceneTasks = loadSceneList.Select(x => SceneManager.LoadSceneAsync(x, LoadSceneMode.Additive).ToUniTask(cancellationToken: token));
            var unloadSceneTasks = unloadSceneList.Select(x => SceneManager.UnloadSceneAsync(x).ToUniTask(cancellationToken: token));
            return UniTask.WhenAll(loadSceneTasks.Concat(unloadSceneTasks));
        }
    }
}