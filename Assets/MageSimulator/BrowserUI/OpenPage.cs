using MageSimulator.BrowserUI.Events;

namespace MageSimulator.BrowserUI
{
    public class OpenPage : BrowserComponent
    {
        public OpenPageEvent args;

        protected override void PassEvent(BrowserEvent e)
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