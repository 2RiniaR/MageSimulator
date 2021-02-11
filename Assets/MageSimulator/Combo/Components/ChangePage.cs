using System;
using System.Collections.Generic;
using MageSimulator.Combo.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace MageSimulator.Combo.Components
{
    public class ChangePage : ComboComponent
    {
        [Serializable]
        private class Match
        {
            public string matchName;
            public ChangePageEvent args;
        }

        public ChangePageEvent defaultArgs;
        [SerializeField] private List<Match> matches;

        protected override void PassEvent(ComboEvent e)
        {
            if (e is SubmitEvent)
            {
                Publish(e.name);
                return;
            }

            PublishEvent(e);
        }

        public void PublishDefault()
        {
            base.PassEvent(defaultArgs);
        }

        public void Publish(string matchName)
        {
            var args = matches.Find(x => x.matchName == matchName)?.args ?? defaultArgs;
            base.PassEvent(args);
        }
    }
}