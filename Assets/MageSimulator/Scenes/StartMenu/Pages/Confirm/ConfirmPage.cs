using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.BrowserUI;
using MageSimulator.Config.Scripts;
using ItemSelector = MageSimulator.BrowserUI.ItemSelector;

namespace MageSimulator.Scenes.StartMenu.Pages.Confirm
{
    public class ConfirmPage : BrowserComponent
    {
        public ItemSelector selector;
        public ApplicationSettings settings;

        private void Awake()
        {
            selector.min = 0;
            selector.max = 1;
            selector.initialValue = settings.allowRecording ? 1 : 0;
        }

        protected override UniTask OnClosePage(Link source, CancellationToken token = new CancellationToken())
        {
            settings.allowRecording = selector.CurrentValue == 1;
            return UniTask.CompletedTask;
        }
    }
}
