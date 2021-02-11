using System;
using MageSimulator.Combo.Utils;

namespace MageSimulator.Combo.Events
{
    [Serializable]
    public class OpenPageEvent : ComboEvent
    {
        public PageTarget target;
    }
}