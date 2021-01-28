using MageSimulator.BrowserUI;
using MageSimulator.Config.Scripts;
using ItemSelector = MageSimulator.BrowserUI.ItemSelector;

namespace MageSimulator.Scenes.StartMenu.Pages.ToNext
{
    public class ToNextPage : BrowserComponent
    {
        public ItemSelector content;
        public ApplicationSettings settings;

        private void Awake()
        {
            content.min = 0;
            content.max = 1;
            content.initialValue = settings.allowRecording ? 1 : 0;
        }
    }
}
