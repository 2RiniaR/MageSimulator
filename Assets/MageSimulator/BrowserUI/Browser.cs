using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MageSimulator.BrowserUI
{
    public class Browser : MonoBehaviour
    {
        public Page initialPage;
        public Transform pageDisplay;
        private GameObject _currentPage;
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelWith(this);

            foreach (var initializer in GetComponents<IBrowserInitializer>())
                initializer.InitializeBrowser(this);

            SetPage(initialPage);
        }

        public async UniTask OpenPage(Page page)
        {
            if (page == null) return;
            _currentPage = Instantiate(page.prefab, pageDisplay);
            await UniTask.WhenAll(BrowserComponent.SearchChildrenComponent(transform)
                .Select(comp => comp.InitializeComponent(this, _cancellationTokenSource.Token)));
        }

        public async UniTask ClosePage()
        {
            if (_currentPage != null)
                Destroy(_currentPage.gameObject);
        }

        public void SetPage(Page target)
        {
            if (target == null) return;
            if (_currentPage != null)
                Destroy(_currentPage.gameObject);
            _currentPage = Instantiate(target.prefab, pageDisplay);

            foreach (var comp in _currentPage.GetComponents<BrowserComponent>())
                comp.InitializeComponent(this, _cancellationTokenSource.Token).Forget();
        }

        public void SetCache(string key, string value)
        {
            if (_cache.ContainsKey(key))
                _cache[key] = value;
            else
                _cache.Add(key, value);
        }

        public string GetCache(string key)
        {
            return _cache.ContainsKey(key) ? _cache[key] : null;
        }

        public void RemoveCache(string key)
        {
            if (!_cache.ContainsKey(key)) return;
            _cache.Remove(key);
        }
    }
}