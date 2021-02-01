using MageSimulator.BrowserUI.Events;

namespace MageSimulator.BrowserUI
{
    public class Finish : BrowserComponent
    {
        public string linkName;

        protected override void PassEvent(BrowserEvent e)
        {
            if (e is SubmitEvent)
                Publish();
            else
                PublishEvent(e);
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