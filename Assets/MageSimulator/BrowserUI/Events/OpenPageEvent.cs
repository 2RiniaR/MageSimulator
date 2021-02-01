using System;

namespace MageSimulator.BrowserUI.Events
{
    [Serializable]
    public class OpenPageEvent : BrowserEvent
    {
        public PageTarget target;
    }
}