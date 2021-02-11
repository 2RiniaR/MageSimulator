using System;
using MageSimulator.Combo.Utils;

namespace MageSimulator.Combo.Events
{
    [Serializable]
    public class ChangePageEvent : FinishPageEvent
    {
        public PageTarget target;
    }
}