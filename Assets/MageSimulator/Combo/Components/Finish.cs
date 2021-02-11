using MageSimulator.Combo.Events;

namespace MageSimulator.Combo.Components
{
    public class Finish : ComboComponent
    {
        public string linkName;

        protected override void PassEvent(ComboEvent e)
        {
            if (e is SubmitEvent)
            {
                PassEventEffects(GetEvent());
                Publish();
                return;
            }

            base.PassEvent(e);
        }

        private FinishPageEvent GetEvent()
        {
            return new FinishPageEvent {name = linkName};
        }

        public void Publish()
        {
            PublishEvent(GetEvent());
        }
    }
}