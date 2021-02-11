using MageSimulator.Combo.Events;

namespace MageSimulator.Combo.Components
{
    public class Submit : ComboComponent
    {
        public SubmitEvent args;

        public void Publish()
        {
            PublishEvent(args);
        }
    }
}