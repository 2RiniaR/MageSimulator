using System.Collections;
using MageSimulator.BrowserUI;

namespace MageSimulator.Talk.Scripts
{
    public class TalkSubmitAction : Submit
    {
        public TalkSection section;
        public TalkDisplay display;

        protected override void Start()
        {
            StartCoroutine(Run());
            base.Start();
        }

        private IEnumerator Run()
        {
            yield return display.RunTalkSection(section);
            Publish();
        }
    }
}