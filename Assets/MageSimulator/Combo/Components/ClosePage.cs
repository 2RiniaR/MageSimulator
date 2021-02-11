using MageSimulator.Combo.Events;

namespace MageSimulator.Combo.Components
{
    public class ClosePage : ComboComponent
    {
        public ClosePageEvent args;

        protected override void PassEvent(ComboEvent e)
        {
            if (e is SubmitEvent)
                Publish();
            else
                PublishEvent(e);
        }

        public void Publish()
        {
            PublishEvent(args);
        }
    }
}