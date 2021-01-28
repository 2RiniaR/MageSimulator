using UnityEngine;

namespace MageSimulator.BrowserUI
{
    [CreateAssetMenu(menuName = "BrowserUI/Page", fileName = "New Page", order = 0)]
    public class Page : ScriptableObject
    {
        public GameObject prefab;
    }
}