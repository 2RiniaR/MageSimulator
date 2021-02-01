using MageSimulator.BrowserUI;
using MageSimulator.Config.Scripts;
using UnityEngine;

namespace MageSimulator.StartMenu.Pages.ToNext
{
    public class ToNextPage : Page
    {
        public GameObject allow;
        public GameObject disallow;
        public ApplicationSettings settings;

        protected override void Start()
        {
            allow.SetActive(settings.allowRecording);
            disallow.SetActive(!settings.allowRecording);

            base.Start();
        }
    }
}
