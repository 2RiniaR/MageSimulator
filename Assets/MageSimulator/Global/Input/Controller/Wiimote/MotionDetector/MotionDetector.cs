using System;
using UniRx;

namespace MageSimulator.Global.Input.Controller.Wiimote.MotionDetector
{
    public class MotionDetector : WiimoteAction
    {
        public IObservable<Unit> OnDetect => _onDetect;
        private readonly Subject<Unit> _onDetect = new Subject<Unit>();

        protected void Detect()
        {
            _onDetect.OnNext(Unit.Default);
        }
    }
}