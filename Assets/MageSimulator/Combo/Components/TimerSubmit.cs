using System;
using UniRx;

namespace MageSimulator.Combo.Components
{
    public class TimerSubmit : Submit
    {
        public float time;

        protected override void Initialize()
        {
            Observable.Timer(TimeSpan.FromSeconds(time)).First().Subscribe(_ => Publish()).AddTo(this);
            base.Initialize();
        }
    }
}