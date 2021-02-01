using System.Threading;
using MageSimulator.BrowserUI.Events;

namespace MageSimulator.BrowserUI
{
    public class Page : BrowserComponent
    {
        private CancellationTokenSource _cancellationTokenSource;

        protected override void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelWith(this);
            base.Start();
        }

        protected override void PassEvent(BrowserEvent e)
        {
            switch (e)
            {
                case OverEvent over:
                    PublishEvent(new SubmitEvent {name = over.name});
                    break;

                case ChangePageEvent link:
                    OpenPage(link.target);
                    ClosePage();
                    break;

                case OpenPageEvent open:
                    OpenPage(open.target);
                    break;

                case ClosePageEvent close:
                    ClosePage();
                    break;
            }
        }

        private void OpenPage(PageTarget pageTarget)
        {
            if (pageTarget == null) return;
            Instantiate(pageTarget.prefab, transform.parent);
        }

        private void ClosePage()
        {
            Destroy(gameObject);
        }
    }
}