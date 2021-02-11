using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Events;
using MageSimulator.Global.Base.Fade.Scripts;
using UnityEngine.SceneManagement;

namespace MageSimulator.Combo.Components
{
    public class SceneLoader : ComboComponent
    {
        public List<string> loadSceneList;
        public List<string> unloadSceneList;
        public string setActiveScene;
        public bool withFade;

        protected override void PassEvent(ComboEvent e)
        {
            if (!(e is SubmitEvent))
                base.PassEvent(e);
            else
                LoadScenes().Forget();
        }

        private async UniTask GetLoadScenesTask(CancellationToken token = new CancellationToken())
        {
            var loadSceneTasks = loadSceneList.Select(x =>
                SceneManager.LoadSceneAsync(x, LoadSceneMode.Additive).ToUniTask(cancellationToken: token));
            var unloadSceneTasks = unloadSceneList.Select(x =>
                SceneManager.UnloadSceneAsync(x).ToUniTask(cancellationToken: token));
            await UniTask.WhenAll(loadSceneTasks.Concat(unloadSceneTasks));
            if (token.IsCancellationRequested) return;
            if (!string.IsNullOrEmpty(setActiveScene))
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(setActiveScene));
        }

        protected UniTask LoadScenes(CancellationToken token = new CancellationToken())
        {
            Func<UniTask> allTasks = () => GetLoadScenesTask(token);
            if (!withFade) return allTasks();
            var fade = FindObjectOfType<Fade>();
            return fade.ProcessWithFade(allTasks, token);
        }
    }
}