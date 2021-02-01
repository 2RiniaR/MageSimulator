﻿using MageSimulator.BrowserUI.Events;

namespace MageSimulator.BrowserUI
{
    public class ChangePage : BrowserComponent
    {
        public ChangePageEvent args;

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