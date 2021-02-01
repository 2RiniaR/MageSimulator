using MageSimulator.BrowserUI.Events;

namespace MageSimulator.BrowserUI
{
    public class Submit : BrowserComponent
    {
        public SubmitEvent args;

        public void Publish()
        {
            PublishEvent(args);
        }
    }
}