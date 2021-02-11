using Cysharp.Threading.Tasks;
using MageSimulator.Combo;
using MageSimulator.Combo.Components;
using MageSimulator.Combo.Events;
using UnityEngine;

namespace MageSimulator.Talk.Scripts.Combo
{
    [RequireComponent(typeof(Talk))]
    public class TalkSubmit : Submit
    {
        public TalkSection section;
        private Talk _display;

        protected override void Start()
        {
            _display = GetComponent<Talk>();
            base.Start();
        }

        protected override void Initialize()
        {
            UniTask.RunOnThreadPool(async () =>
            {
                await _display.RunSection(section);
                Publish();
            }).Forget();

            base.Initialize();
        }
    }
}