using System;

namespace MageSimulator.BrowserUI.Events
{
    [Serializable]
    public class ChangePageEvent : FinishPageEvent
    {
        public PageTarget target;
    }
}