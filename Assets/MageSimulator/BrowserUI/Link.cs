using System.Threading;
using Cysharp.Threading.Tasks;

namespace MageSimulator.BrowserUI
{
    public abstract class Link : BrowserComponent
    {
        public Page target;
        public string linkName;
        public const string CacheKey = "PageLinkName";
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelWith(this);
        }

        protected void Request()
        {
            RequestClosePage(this, async browser =>
            {
                browser.SetCache(CacheKey, linkName);
                await browser.OpenPage(target);
            }, _cancellationTokenSource.Token).Forget();
        }
    }
}