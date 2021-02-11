using System.Threading;
using MageSimulator.Combo.Events;
using MageSimulator.Combo.Utils;
using MageSimulator.Utils.Scripts;

namespace MageSimulator.Combo.Components
{
    public class Page : ComboComponent
    {
        private CancellationTokenSource _cancellationTokenSource;

        protected override void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelWith(this);
            base.Start();
        }

        protected override void PassEvent(ComboEvent e)
        {
            switch (e)
            {
                case ReturnPageEvent over:
                    var newEvent = new SubmitEvent {name = over.name};
                    base.PassEvent(newEvent);
                    break;

                case ChangePageEvent link:
                    PassEventEffects(e);
                    OpenPage(link.target);
                    ClosePage();
                    break;

                case OpenPageEvent open:
                    PassEventEffects(e);
                    OpenPage(open.target);
                    break;

                case ClosePageEvent close:
                    PassEventEffects(e);
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