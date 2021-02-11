using MageSimulator.Combo.Events;

namespace MageSimulator.Combo.Components
{
    public class OpenPage : ComboComponent
    {
        public OpenPageEvent args;

        protected override void PassEvent(ComboEvent e)
        {
            if (e is SubmitEvent)
            {
                PassEventEffects(args);
                Publish();
                return;
            }

            PublishEvent(e);
        }

        public void Publish()
        {
            PublishEvent(args);
        }
    }
}