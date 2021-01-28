using UnityEngine;

namespace MageSimulator.BrowserUI
{
    public class LinkInitializer : MonoBehaviour, IBrowserInitializer
    {
        public string initialAnimation;

        public void InitializeBrowser(Browser browser)
        {
            browser.SetCache(Link.CacheKey, initialAnimation);
        }
    }
}