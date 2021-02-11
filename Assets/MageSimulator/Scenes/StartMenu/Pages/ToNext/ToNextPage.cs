using MageSimulator.Combo.Components;
using MageSimulator.Global.Setting;
using UnityEngine;

namespace MageSimulator.Scenes.StartMenu.Pages.ToNext
{
    public class ToNextPage : MonoBehaviour
    {
        public GameObject allow;
        public GameObject disallow;
        public ApplicationSettings settings;

        private void Start()
        {
            allow.SetActive(settings.allowRecording);
            disallow.SetActive(!settings.allowRecording);
        }
    }
}
